// use an in-game timer, the speedrun is essentially RTA from that point
// use GETREALTIME. NOT GETSIMTIME. getSimTime stops when the game lags at ANY point!

// GAMEMODE IS VALID FOR THE FOLLOWING CATEGORIES:
// - v21 Tutorial Any%
// - v21 Tutorial Any% No OoB
// - v21 Tutorial 100%

$SpeedunVersion = "4-v21";

if(!$Server::LAN) {
	error("You cannot host this gamemode on an internet server.");
	messageAll('MsgAdminForce',"You cannot host this gamemode on an internet server.");
	return;
}

if($GameModeArg !$= "Add-Ons/GameMode_Tutorial_Speedrun/gamemode.txt") {
	error("You must not have any other mods running to speedrun the tutorial.");
	messageAll('MsgAdminForce',"You must not have any other mods running to speedrun the tutorial.");
	return;
}

function displayInformation() {
	// displaying some information to prevent "YOU CHEATED" crap

	// assuming there might be a better way, trace showed nothing relating to setting it
	messageAll('',"\c2BL VERSION\c6: v" @ $version SPC "\c6r" @ getWord(MM_Version.getValue(),1));

	if(isFile("Add-Ons/Gamemode_Tutorial_Speedrun.zip")) {
		messageAll('',"\c2GAMEMODE CRC [ZIP]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun.zip"));
		%zip = 1;
	}
	if(isFile("Add-Ons/Gamemode_Tutorial_Speedrun/server.cs") && !%zip) {
		messageAll('',"\c2GAMEMODE CRC [SERVER.CS | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/server.cs"));
		messageAll('',"\c2GAMEMODE CRC [CLIENT.CS | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/client.cs"));
		messageAll('',"\c2GAMEMODE CRC [SPEEDRUN.CS | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/speedrun.cs"));
		messageAll('',"\c2GAMEMODE CRC [CREATETRIGGERS.CS | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/createTriggers.cs"));
		messageAll('',"\c2GAMEMODE CRC [SAVE FILE | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/save.bls"));
		messageAll('',"\c2GAMEMODE CRC [NAMECHECK | FOLDER]\c6:" SPC getFileCRC("Add-Ons/Gamemode_Tutorial_Speedrun/namecheck.txt"));
	}

	// version 1: ###
	messageAll('',"\c2DATABLOCKS\c6:" SPC DatablockGroup.getCount());

	messageAll('',"\c2GAMEMODE VERSION\c6:" SPC $SpeedunVersion);

	messageAll('',"\c2DATETIME\c6:" SPC getDateTime());

	messageAll('',"\c2ATTEMPT\c6:" SPC $Speedrun::Attempts);
}
messageAll('',"\c5Allowing the player to ghost bricks. Resetting in 10 seconds... (Be sure to set your max chat lines to 13 for run verification!)");

function GameConnection::doTimer(%this) {
	if(isEventPending($DefaultMinigame.resetSchedule)) {
		%time = getTimeRemaining($DefaultMinigame.resetSchedule);
	} else {
		%time = getRealTime() - %this.startTime[real];
	}

	cancel(%this.timerLoop);
	%this.timerLoop = %this.schedule(1,doTimer);

	%this.bottomPrint("<just:center>\c6" @ getTimeString(%time/1000));
}

package TutorialSpeedrunPackage {
	function Player::CompleteTutorial(%this) {
		if($Speedrun::Completed) {
			return;
		}
		%client = %this.client;
		%client.centerPrint("\c5Restarting in 15 seconds...",5);
		if(isEventPending(%client.timerLoop)) {
			cancel(%client.timerLoop);
		}
		messageAll('MsgAdminForce',"<font:Arial Bold:36>\c4SIM TIME (\"IN-GAME\")\c6:" SPC getTimeString((getSimTime() - %client.startTime[sim])/1000));
		messageAll('',"<font:Arial Bold:36>\c4REAL TIME (\"RTA\")\c6:" SPC getTimeString((getRealTime() - %client.startTime[real])/1000));
		$DefaultMinigame.resetSchedule = $DefaultMinigame.schedule(15000,respawnAll);
		$Speedrun::Completed = 1;
	}
	function GameConnection::spawnPlayer(%this) {
		// "in-game" timer
		%this.startTime[sim] = getSimTime();
		// "RTA" timer
		%this.startTime[real] = getRealTime();

		%this.doTimer();
		$Speedrun::Attempts++;
		displayInformation();
		$Speedrun::Completed = 0;

		for(%i=0;%i<BrickGroup_888888.getCount();%i++) { 
			%brick = BrickGroup_888888.getObject(%i);
			%tut_bricks = "_brickDoor _build1Door _build2Door _lightDoor _roomBreak";
			if(stripos(%tut_bricks,%brick.getName()) != -1) {
				%brick.fakeKillBrick("0 0 0",1);
				%brick.setRaycasting(1);
				%brick.setColliding(1);
				%brick.setRendering(1);
			}
			if(%brick.getDatablock().getName() $= "brickVehicleSpawnData") {
				%brick.respawnVehicle();
			}
			%chests = 0;
			if(%brick.getDatablock().getName() $= "brickTreasureChestData") {
				%hash = sha1(%brick.getPosition() @ %brick.angleID);
				%this.foundTreasureChest_[%hash] = "";
				%this.foundTreasureChestList[%chests] = "";
				%chests++;
			}
		}
		BrickGroup_999999.deleteAll();
		$LoadingBricks_Silent = true;
		$LoadingBricks_StartTime = getSimTime()+2000;
		schedule(2000,0,ServerLoadSaveFile_Start,"Add-Ons/Gamemode_Tutorial_Speedrun/userBricks.bls");

		return parent::spawnPlayer(%this);
	}
};
activatePackage(TutorialSpeedrunPackage);