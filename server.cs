exec("./speedrun.cs");

if( $GameModeArg !$= "Add-Ons/Gamemode_Tutorial_Speedrun/gamemode.txt" )
{
	error( "Error: Gamemode_Tutorial_Speedrun cannot be used in custom games" );
	return;
}

function saveTriggers()
{
   //loop through mission cleanup, find trigger objects, move them to tutorial group
   //save tutorial group to file

   //create tutorial group if it does not exist
   if(!isObject(TutorialGroup))
   {
      new SimGroup(TutorialGroup);
      MissionCleanup.add(TutorialGroup);
   }

   %group = "MissionCleanup";
   %count = %group.getCount();
   for(%i = 0; %i < %count; %i++)
   {
      %obj = %group.getObject(%i);
      if(%obj.getClassName() !$= "Trigger" &&
         %obj.getClassName() !$= "Marker" )
      {
         continue;
      }
   
      TutorialGroup.add(%obj);
   }

   %filename = "Add-Ons/GameMode_Tutorial/createTriggers.cs";
   TutorialGroup.save(%filename);
}


//-----------------------------------
// win trigger
//  for detecting when you've completed an obstacle
//-----------------------------------
datablock TriggerData(TutorialWinTrigger)
{
   tickPeriodMS = 100;
};

function TutorialWinTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);
   
   if(isObject(%obj.getMountedObject(0).client))
   {
      %obj = %obj.getMountedObject(0);
   }

   //hack for escaping jet room.  buildwin trigger should probably be it's own thing
   if(%trigger.goalName $= "Build")
   {
      if(isObject(%obj.client))
         %obj.setDataBlock(PlayerTutorialNoJet);   
   }

   if($TutorialCompleted && %trigger.goalName !$= "Secrets")
      return;
   
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;
   if(%obj.goalCompleted[%trigger.goalName] == true)
      return;
      
   //hacks for enabling player abilities
   switch$(%trigger.goalName)
   {
      case "Diving":
         %obj.setDataBlock(PlayerTutorialNoJet);   
      case "Ride":
         _electricWater.setEmitter(0);
      case "Build":         
         if(isObject(%client)) //make client put away brick
         {
            %obj.unMountImage(0);
            fixArmReady(%obj);
            commandToClient(%client, 'SetScrollMode', 3);                        
         }
   }

   if(%trigger.goalName $= "Light")
   {
      if(isObject(%client.player.light))
         serverCmdLight(%client);
      
      if(Sun.oldColor !$= "")
      {
         for(%i = 0; %i < 10; %i++)
         {
            %factor = %i / 10;
            schedule(%i * 32, Sun, tutorialSetSunValues, vectorScale(Sun.oldColor, %factor), vectorScale(Sun.oldAmbient, %factor), vectorScale(Sun.oldShadowColor, %factor));
         }
         schedule(%i * 32, Sun, tutorialSetSunValues, Sun.oldColor, Sun.oldAmbient, Sun.oldShadowColor);
         Sun.oldColor = "";
         Sun.oldAmbient = "";
         Sun.oldShadowColor = "";
      }
   }

   %obj.CompleteTutorialGoal(%trigger.goalName);

  
}

function Player::CompleteTutorialGoal(%player, %goalName)
{
   %client = %player.client;

   if(%player.goalCompleted[%goalName] == true)
      return;

   %player.goalCompleted[%goalName] = true;

   %time = getTimeString((getSimTime() - %player.spawnTime) / 1000);
   %player.emote(winStarProjectile, 1); //1 = skip spam protection
   %client.play2D(rewardSound);
   $Tutorial::GoalsCompleted++;
   CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - " @ %goalName @ " - Time: " @ %time, 3);
   CommandToClient(%client, 'CenterPrint', "", 0);
}
function Player::CompleteTutorial(%player)
{
   %client = %player.client;
   //$TutorialCompleted = true;
   commandToClient(%client, 'TutorialCompleted');
   %player.CompleteTutorialGoal("Treasure");
}

function TutorialWinTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialWinTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
}

//-----------------------------------
// generic tip trigger
//  simple "press x to <verb>" tips
//-----------------------------------
datablock TriggerData(TutorialTipTrigger)
{
   tickPeriodMS = 100;
};

function TutorialTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);

   if($TutorialCompleted)
      return;

   //%this.giveTip(%trigger, %obj);      
}

function TutorialTipTrigger::GiveTip(%this, %trigger, %obj)
{  
   %obj.lastTipTime = getSimTime();
   
   %client = %obj.client;

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to " @ %trigger.taskName , 2);
}

function TutorialTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);

   if(%obj.goalCompleted[%trigger.goalName] == true || $TutorialCompleted)
      return;

   switch$(%trigger.goalName)
   {
      case "Diving":
         %obj.setDataBlock(PlayerTutorialNoJet);   
   }
}

function TutorialTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);

   if($TutorialCompleted)
      return;

   if(isObject(%obj.getMountedObject(0).client))
   {
      %obj = %obj.getMountedObject(0);
   }
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   if(%obj.getDamageLevel() >= 1.0)
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   switch$(%trigger.goalName)
   {
      case "Jet":
         %obj.setDataBlock(PlayerStandardArmor);          
      case "Light":
         %obj.setDataBlock(PlayerTutorialNoJet);
         if(isObject(%obj.light))
         {
            openTutorialDoor("lightDoor");
         }  
   }

   if(%obj.goalCompleted[%trigger.goalName] == true)
      return;

   //hacks for enabling player abilities
   switch$(%trigger.goalName)
   {
      case "Jump":
         %obj.setDataBlock(PlayerTutorialNoJet);   
      case "Light":
         if(!$LoadingBricks_Client)
         {
            //add restrictions to the vehicle pads
            _TutorialVehicleBrick1.vehicleLimit = "HorseArmor";
            _TutorialVehicleBrick2.vehicleLimit = "JeepVehicle";

            //prevent breaking of certain key bricks
            _TutorialVehicleBrick1.noBreak    = true;
            _TutorialVehicleBrick2.noBreak    = true;

            _TutorialWrenchBrick.noBreak      = true;
            
            _TutorialWrenchSpawnBrick.noBreak  = true;
            _TutorialPrinterSpawnBrick.noBreak = true;
            _TutorialGunSpawnBrick.noBreak     = true;

            _TutorialPrintBrick1.noBreak = true;
            _TutorialPrintBrick2.noBreak = true;
            _TutorialPrintBrick3.noBreak = true;
            _TutorialPrintBrick4.noBreak = true;
            _TutorialPrintBrick5.noBreak = true;
            _TutorialPrintBrick6.noBreak = true;
            _TutorialPrintBrick7.noBreak = true;

            _TutorialDoor2.noBreak = true;
            _TutorialDoor3.noBreak = true;
            _TutorialDoor4.noBreak = true;
            _TutorialDoor5.noBreak = true;
            _TutorialDoor6.noBreak = true;
         }

         if(isObject(%client.player.light))
         {
            %obj.lastTipTime = 0;
            commandToClient(%client, 'centerPrint');
            return;
         }
      case "Dismount":
         if(!isObject(%client.player.getObjectMount()))
         {
            %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);

            %obj.emote(winStarProjectile, 1); //1 = skip spam protection
            %client.play2D(rewardSound);
            $Tutorial::GoalsCompleted++;
            CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Dismount - Time: " @ %time, 3);
            CommandToClient(%client, 'CenterPrint', "", 0);

            %obj.goalCompleted["Dismount"] = 1;
            return;
         }
      case "Diving":
         %obj.setDataBlock(PlayerStandardArmor);  
   }
   

   if( (getSimTime() - %obj.lastTipTime) > 2500 )
   {
      %this.giveTip(%trigger, %obj);
   }
}


//-----------------------------------
// Look tip trigger
//  Hackish thing to prompt you to look behind you and then move
//-----------------------------------

datablock TriggerData(TutorialLookTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialLookTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if($TutorialCompleted)
      return;

   if(!%obj.goalCompleted["Look"])
   {
      //stop player from moving 
      %obj.setDataBlock(PlayerTutorialNoMove);

      //clear inventory
      messageClient(%client, 'MsgClearInv');
      commandToClient(%client, 'PlayGui_CreateToolHud', %obj.getDataBlock().maxTools);
      for(%i = 0; %i < 5; %i++)
      {
         %obj.tool[%i] = 0;
      }
      %obj.weaponCount = 0;

      for(%i = 0; %i < 10; %i++)
      {
         %obj.client.inventory[%i] = 0;
      }

      stopTargetPractice();
      if(isObject(TutorialTargets))
         TutorialTargets.delete();

      //clear bricks and load the default save
      $TutorialPart1Completed = false;
      $Tutorial::GoalsCompleted = 0;
      $Tutorial::TotalTargetsHit = 0;
      $Tutorial::TotalTargetsLaunched = 0;

      //reset electric water effect
      _electricWater.setEmitter(radioWaveExplosionEmitter);
          
      $Tutorial::HorseExists = 0;
      $Tutorial::Horse = "";    

      //prompt them too look behind them
      %obj.lastTipTime = getSimTime();
      CommandToClient(%client, 'CenterPrint', "Move \c3the mouse\c0 to look around", 2);
   }  
   else if(!%obj.goalCompleted["Move"])
   {
      //tell them to how to move
      %obj.lastTipTime = getSimTime();
   }
   
}

function openTutorialDoor(%doorName)
{
   //find all bricks that match name, blow them up
   %name = "_" @ %doorName;
   %group = BrickGroup_888888;
   %count = %group.NTObjectCount[%name];
//   echo("opening " @ %count @ " bricks");
   for(%i = 0; %i < %count; %i++)
   {
      %obj = %group.NTObject[%name, %i];

      if(!%obj.isRendering())
         continue;
      if(%obj.isFakeDead())
         continue;
      
      %x = getRandom() * 14 - 7;
      %y = getRandom() * 14 - 7;
      %z = 5;
      // changed from 30 to 10, this too appears to be causing issues
      %obj.fakeKillBrick(%x SPC %y SPC %z, 10);
      // disabled, this is causing issues in runs
      // why this is even here is beyond me
      //%obj.schedule(35000, setRendering, 0);
      //%obj.schedule(35000, setRaycasting, 0);
      //%obj.schedule(35000, setColliding, 0);
   }

}

function TutorialLookTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialLookTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(!%obj.goalCompleted["Look"])
   {
      //check if they're looking the right direction
         //if they are, complete goal and let them move again
      %targetPos = TutorialLookTarget.getPosition();
      %playerPos = %obj.getPosition();
      %targetVec = vectorNormalize(vectorSub(%targetPos, %playerPos));
      %eyeVec = %obj.getEyeVector();

      %dot = vectorDot(%targetVec, %eyeVec);

      if(%dot > 0.65)
      {
         //reward
         %obj.goalCompleted["Look"] = true;
         %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
         %obj.emote(winStarProjectile, 1); //1 = skip spam protection
         %client.play2D(rewardSound);
         $Tutorial::GoalsCompleted++;
         CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Look - Time: " @ %time, 3);

         //allow movement
         %obj.setDataBlock(PlayerTutorialNoJumpNoJet);
         return;
      }

      //prompt them too look behind them
      if(getSimTime() - %obj.lastTipTime > 2500)
      {
         %obj.lastTipTime = getSimTime();
         CommandToClient(%client, 'CenterPrint', "Move \c3the mouse\c0 to look around", 2);
      }
   }  
   else if(!%obj.goalCompleted["Move"])
   {
      //tell them to how to move
      if(getSimTime() - %obj.lastTipTime > 2500)
      {
         %obj.lastTipTime = getSimTime();

         //need bind name, task name, goal name 
         %wkey = strUpr(getField(movemap.getBinding(moveforward), 1));
         %akey = strUpr(getField(movemap.getBinding(moveleft), 1));
         %skey = strUpr(getField(movemap.getBinding(movebackward), 1));         
         %dkey = strUpr(getField(movemap.getBinding(moveright), 1));

         CommandToClient(%client, 'CenterPrint', "Press \c3" @ %wkey SPC %akey SPC %skey SPC %dkey  @ "\c0 to move" , 2);
      }
   }

}

//-----------------------------------
// Brick tip trigger
//  Hackish thing to detect when you buy bricks
//-----------------------------------

datablock TriggerData(TutorialBrickTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialBrickTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialBrickTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialBrickTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Brick"])
      return;

   if(%obj.getMountedImage(0) $= brickImage.getID())
   {
      %obj.goalCompleted["Brick"] = true;
	   openTutorialDoor("brickDoor");

      %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
      
      %obj.emote(winStarProjectile, 1); //1 = skip spam protection
      %client.play2D(rewardSound);
      $Tutorial::GoalsCompleted++;
      CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Bricks - Time: " @ %time, 3);
      CommandToClient(%client, 'CenterPrint', "", 0);
      return;
   }

   //see if we have bricks
   %maxInv = %obj.getDataBlock().maxItems;
   for(%i = 0; %i < %maxInv; %i++)
   {
      if(isObject(%obj.client.inventory[%i]))
      {
         %obj.goalCompleted["Brick"] = true;
	      openTutorialDoor("brickDoor");

         %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
      
         %obj.emote(winStarProjectile, 1); //1 = skip spam protection
         %client.play2D(rewardSound);
         $Tutorial::GoalsCompleted++;
         CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Bricks - Time: " @ %time, 3);
         CommandToClient(%client, 'CenterPrint', "", 0);
         return;
      }
   }

   if(getSimTime() - %obj.lastTipTime < 2500)
      return;

   %obj.lastTipTime = getSimTime();

   //enable building
   $defaultMiniGame.setEnableBuilding(1);

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to " @ %trigger.taskName , 2);

}

//-----------------------------------
// Build tip trigger
//  shoot brick, plant
//-----------------------------------

datablock TriggerData(TutorialBuildTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialBuildTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialBuildTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   if(!$TutorialCompleted)
   {
      if(%obj.getMountedImage(0).item $= "sprayCan")
	   %obj.unmountImage(0);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialBuildTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Build1"])
      return;

   //need bind name, task name, goal name 
   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;

   if(isObject(%obj.tempBrick))
   {
      %plantkey = bindNameFix("plantBrick");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %plantkey @ "\c0 to plant brick", 3);
   }
   else if(%image == BrickImage.getId())
   {
      %key = bindNameFix("mouseFire");
      CommandToClient(%client, 'CenterPrint', "Look at the platform and press \c3" @ %key @ "\c0 to create a ghost brick" , 2);
   }   
   else
   {
      %key = bindNameFix("useBricks");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip bricks" , 2);
   }

}


//-----------------------------------
// Build tip trigger B
//  shoot brick, maneuver it, then plant
//-----------------------------------

datablock TriggerData(TutorialBuildTipTriggerB)
{
   tickPeriodMS = 50;
};

function TutorialBuildTipTriggerB::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
   if(!%obj.startedBuildB)
   {
      %obj.startedBuildB = true;
      if(isObject(%obj.tempBrick))
      {
         %obj.tempBrick.delete();
         %obj.tempBrick = 0;
      }

      %obj.completeTutorialGoal("Build1");
   }
}

function TutorialBuildTipTriggerB::onLeaveTrigger(%this,%trigger,%obj)
{
   if(!$TutorialCompleted)
   {
      if(%obj.getMountedImage(0).item $= "sprayCan")
	   %obj.unmountImage(0);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialBuildTipTriggerB::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Build2"])
      return;

   //need bind name, task name, goal name 
   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;

   if(isObject(%obj.tempBrick))
   {
      //if brick is in position, tell them to plant
      //if brick not in position, tell them to maneuver it
      %pos = %obj.tempBrick.getPosition();
      %x = getWord(%pos, 0);
      %y = getWord(%pos, 1);
      %z = getWord(%pos, 2);

      //echo("z = " @ %z);

      if(mabs(%x -  -1.5) < 0.01 && //compare with subtraction to avoid floating point errors
         mabs(%y - -10.5) < 0.01 && 
         mabs(%z -  17.5) < 0.01 && 
         (%obj.tempBrick.angleId == 1 || %obj.tempBrick.angleId == 3)  )
      {
         %plantkey = bindNameFix("plantBrick");
         CommandToClient(%client, 'CenterPrint', "Press \c3" @ %plantkey @ "\c0 to plant brick", 3);
      }
      else if(mabs(%z -  17.5) > 0.01)
      {
         //need to move vertically
         %sukey = bindNameFix("shiftBrickUp");
         %sdkey = bindNameFix("shiftBrickDown");
         %sutkey = bindNameFix("shiftBrickThirdUp");
         %sdtkey = bindNameFix("shiftBrickThirdDown");
         CommandToClient(%client, 'CenterPrint', "Move the brick into position.\n\n\c0Press \c3" @ %sukey @"\c0,\c3 "@ %sdkey  @"\c0,\c3 "@ %sutkey @ " \c0or\c3 " @ %sdtkey @ " \c0to move brick vertically", 3);
      }
      else if(mabs(%x -  -1.5) > 0.01 || mabs(%y - -10.5) > 0.01)
      {
         //need to move horizontally
         %sakey = bindNameFix("shiftBrickAway");
         %stkey = bindNameFix("shiftBrickTowards");
         %slkey = bindNameFix("shiftBrickLeft");
         %srkey = bindNameFix("shiftBrickRight");
         CommandToClient(%client, 'CenterPrint', "Move the brick into position.\n\nPress \c3" @ %sakey @"\c0,\c3 "@ %stkey @"\c0,\c3 "@ %slkey @" \c0or\c3 "@ %srkey @ "\c0 to move brick laterally", 3);
      }
      else if(%obj.tempBrick.angleId == 0 || %obj.tempBrick.angleId == 2)
      {
         //need to rotate
         %cwkey = bindNameFix("RotateBrickCW");
         %ccwkey = bindNameFix("RotateBrickCCW");

          CommandToClient(%client, 'CenterPrint', "Press \c3" @ %cwkey @"\c0 or \c3 "@ %ccwkey @ "\c0 to rotate the brick", 3);
      }
   }
   else if(%image == BrickImage.getId())
   {
      %key = bindNameFix("mouseFire");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to create a ghost brick" , 2);
   }   
   else
   {
      %key = bindNameFix("useBricks");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip bricks" , 2);
   }
}



//-----------------------------------
// Build tip trigger C
//  build a staircase
//-----------------------------------

datablock TriggerData(TutorialBuildTipTriggerC)
{
   tickPeriodMS = 50;
};

function TutorialBuildTipTriggerC::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  

   %obj.completeTutorialGoal("Build2");
}

function TutorialBuildTipTriggerC::onLeaveTrigger(%this,%trigger,%obj)
{
   if(!$TutorialCompleted)
   {
      if(%obj.getMountedImage(0).item $= "sprayCan")
	   %obj.unmountImage(0);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialBuildTipTriggerC::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Build"])
      return;

   CommandToClient(%client, 'CenterPrint', "Build a staircase to proceed" , 2);
}



//-----------------------------------
// Light start trigger
//  makes everything get dark
//-----------------------------------

datablock TriggerData(TutorialLightStartTrigger)
{
   tickPeriodMS = 50;
};

function TutorialLightStartTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  

   if(%obj.goalCompleted["Light"])
      return;
   
   if(Sun.oldColor $= "")
   {
      Sun.oldColor       = sun.color;
      Sun.oldAmbient     = sun.ambient;
      Sun.oldShadowColor = sun.shadowColor;

      for(%i = 0; %i < 10; %i++)
      {
         %factor = (10 - %i) / 10;
         schedule(%i * 32, Sun, tutorialSetSunValues, vectorScale(Sun.color, %factor), vectorScale(Sun.ambient, %factor), vectorScale(Sun.shadowColor, %factor));
      }
      schedule(%i * 32, Sun, tutorialSetSunValues, "0 0 0", "0 0 0", "0 0 0");
   }
}

function tutorialSetSunValues(%color, %ambient, %shadowColor)
{
   Sun.color       = %color;
   Sun.ambient     = %ambient;
   Sun.shadowColor = %shadowColor;
   Sun.sendUpdate();
}

function TutorialLightStartTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   if(!$TutorialCompleted)
   {
      if(%obj.getMountedImage(0).item $= "sprayCan")
	   %obj.unmountImage(0);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialLightStartTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Light"])
      return;

}

//-----------------------------------
// Break tip trigger
//  Hackish thing to detect when you have a hammer, have it out, etc
//-----------------------------------

datablock TriggerData(TutorialBreakTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialBreakTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialBreakTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialBreakTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;
   
   //to fix backtracking from jet room
   %obj.setDataBlock(PlayerTutorialNoJet);   
   
   if(%obj.goalCompleted["Break"])
      return;

   //need bind name, task name, goal name 
   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;

   if(%image == hammerImage.getID())
   {
      %key = bindNameFix("mouseFire");
      CommandToClient(%client, 'CenterPrint', "Aim at the bricks and press \c3" @ %key @ "\c0 to break them" , 2);
   }
   else if(isObject(%obj.tool[0]))
   {
      %key = bindNameFix("useTools");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip the hammer" , 2);
   }   
   else
   {
      CommandToClient(%client, 'CenterPrint', "Run over the hammer to pick it up" , 2);
   }

}



//-----------------------------------
// Ride tip trigger
//  Hackish thing to detect when you spawn a horse
//-----------------------------------

datablock TriggerData(TutorialRideTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialRideTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   if($TutorialCompleted)
      return;

   if(%obj.dataBlock $= "HorseArmor")
   {
      $Tutorial::HorseExists = 1;
      $Tutorial::Horse = %obj;
      $Tutorial::HorseBrick = %obj.spawnBrick;
   }
   
   //turn electric water back on if player enters who has not completed ride tutorial
   if(isObject(%obj.client) && %obj.goalCompleted["Ride"])
   {
      _electricWater.setEmitter(radioWaveExplosionEmitter);
   }

   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialRideTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   if($TutorialCompleted)
      return;

   if(%obj.dataBlock $= "HorseArmor")
      $Tutorial::HorseExists = 0;
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialRideTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
   
   if($TutorialCompleted)
      return;

   if(%obj.getClassName() $= "WheeledVehicle" || %obj.getClassName() $= "FlyingVehicle")
   {
      %obj.finalExplosion();      
      return;
   }   

   if(isObject(%obj.getMountedObject(0).client))
   {
      %client = %obj.getMountedObject(0).client;
      %obj = %client.player;
   }
   else
      %client = %obj.client;
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Ride"])
      return;

   //if(getSimTime() - %obj.lastTipTime < 500)
   //   return;

   %obj.lastTipTime = getSimTime();

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;
      
   for(%i=0;%i<%obj.dataBlock.maxTools;%i++)
   {
      if(%obj.tool[%i] == WrenchItem.getID())
      {
         %hasWrench = 1;
         break;
      }
   }

   if(isObject(%obj.getObjectMount()))
   {
      CommandToClient(%client, 'CenterPrint', "Control this like you would a Normal Player.\n\nNow jump across the gap to the next area of the Tutorial", 0.7);
   }
   else 
   {
      %key = bindNameFix("Jump");
      CommandToClient(%client, 'CenterPrint', "Jump on top of the horse by pressing \c3"@%key, 0.7);
   }
}



//-----------------------------------
// Water trigger
//  Puts you at beginning of area if you jump in water
//-----------------------------------

datablock TriggerData(TutorialWaterTrigger)
{
   tickPeriodMS = 50;
};

function TutorialWaterTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   if($TutorialCompleted)
      return;

   if(%obj.goalCompleted["Ride"])
      return;
      
   if(isObject(%obj.client))
   {
      //player jumped into electric water, move them back to the shore
      %obj.schedule(200, setTransform, "-37 19.2 16.1");//"-2.64172 -127.042 94.75 0 0 -1 0.881075");
      %obj.schedule(200, setDamageFlash, 0.8);
      %obj.schedule(200, setVelocity, "0 0 0.1");
       
      //respawn the horse
      if(isObject($Tutorial::HorseBrick))
      {
         $Tutorial::HorseBrick.schedule(200, respawnVehicle);
      }  
   }
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialWaterTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialWaterTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
}




//-----------------------------------
// Secrets trigger
//  Tip trigger
//-----------------------------------

datablock TriggerData(TutorialSecretTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialSecretTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialSecretTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialSecretTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Secrets"])
      return;
      
   if(getSimTime() - %obj.lastTipTime < 300)
      return;
      
   %obj.lastTipTime = getSimTime();
      
   centerPrint(%obj.client,"Many of the maps included in Blockland contain \n\c3secret passages\c0 and \c3hidden spaces\c0 to build.\n\nTry finding some of them with your \c3friends\c0 later on",2);
   Parent::onTickTrigger(%this,%trigger);
}



//-----------------------------------
// Wrench tip trigger
//  Hackish thing to detect when you set emitters/items/lights
//-----------------------------------

datablock TriggerData(TutorialWrenchTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialWrenchTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialWrenchTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialWrenchTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if($TutorialCompleted)
      return;

   if(%obj.goalCompleted["Wrench"])
      return;

   //if(getSimTime() - %obj.lastTipTime < 300)
   //   return;

   %obj.lastTipTime = getSimTime();

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;
      
   for(%i=0;%i<%obj.dataBlock.maxTools;%i++)
   {
      if(%obj.tool[%i] == WrenchItem.getID())
      {
         %hasWrench = 1;
         break;
      }
   }
   
   if(getSimTime() - %obj.wrenchDelayTime < 0)
      return;
   
   if(!isObject($Tutorial::WrenchBrick))
      $Tutorial::WrenchBrick = _TutorialWrenchBrick.getId();

   if( (%image $= 0 || %image == BrickImage.getID()) && %hasWrench)
   {
      %key = bindNameFix("useTools");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip the wrench" ,0.5);
   }
   else if(%image !$= WrenchImage.getID() && %hasWrench)
   {
	   CommandToClient(%client, 'CenterPrint', "Use the \c3mouse wheel\c0 to scroll to the wrench" ,0.5);
   }
   else if(!%hasWrench)
   {
      CommandToClient(%client, 'CenterPrint', "Run over the wrench to pick it up" , 0.5);
   }
   else 
   {
      if(!isObject($Tutorial::WrenchBrick.emitter))
      {
         CommandToClient(%client, 'CenterPrint', "Click on the cone brick.\nApply an \c3emitter\c0 to it and press send" , 3);	
      }
      else
      {
         %obj.CompleteTutorialGoal("Wrench");
         openTutorialDoor("wrenchDoor");
      }
   }

   
}



//-----------------------------------
// Print tip trigger
//  Hackish thing to detect when you change a print
//-----------------------------------

datablock TriggerData(TutorialPrintTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialPrintTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{      
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialPrintTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialPrintTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if($TutorialCompleted)
      return;

   if(%obj.goalCompleted["Print"])
      return;

   if(getSimTime() - %obj.lastTipTime < 300)
      return;

   %obj.lastTipTime = getSimTime();

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;
      
   %completedAllBricks = 1;
   %brickPrint1 = $printNameTable["Letters/O"];
   %brickPrint2 = $printNameTable["Letters/I"];
   %brickPrint3 = $printNameTable["Letters/N"];
   %brickPrint4 = $printNameTable["Letters/K"];
   %brickPrint5 = $printNameTable["Letters/M"];
   %brickPrint6 = $printNameTable["Letters/O"];
   %brickPrint7 = $printNameTable["Letters/O"];
   for(%i=1;%i<8;%i++)
   {
      %object = _TutorialPrintBrick@%i;
      if(isObject(%object))
      {
         if(%object.getPrintID() $= %brickPrint[%i])
            %object.setColor(2);
         else if(%object.getPrintID() $= $printNameTable["Letters/-minus"])
         {
            %object.setColor(4);
            %completedAllBricks = 0;
         }
         else
         {
            %object.setColor(0);
            %completedAllBricks = 0;
         }
      }
   }
      
   for(%i=0;%i<%obj.dataBlock.maxTools;%i++)
   {
      if(%obj.tool[%i] == PrintGun.getID())
      {
         %hasPrinter = 1;
         break;
      }
   }
   if( (%image $= 0 || %image == BrickImage.getID()) && %hasPrinter)
   {
      %key = bindNameFix("useTools");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip the printer" ,0.5);
      return;
   }
   else if(%image !$= PrintGunImage.getID() && %hasPrinter)
   {
      %key = bindNameFix("useTools");
      CommandToClient(%client, 'CenterPrint', "Use the \c3mouse wheel\c0 to scroll to the printer" ,0.5);
      return;
   }
   else if(!%hasPrinter)
   {
      CommandToClient(%client, 'CenterPrint', "You need to find a Printer to Continue" , 0.5);
      return;
   }
   else if(!%completedAllBricks)
   {
      CommandToClient(%client, 'CenterPrint', "Shoot a Printable Brick to set its Print\nYou can press the letter or number on your keyboard instead of selecting it\n\n<color:FFFFFF>Complete the Puzzle!" , 0.5);
   }
   else
   {
	openTutorialDoor(3);
      %obj.goalCompleted["Print"] = true;
      %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
      
      %obj.emote(winStarProjectile, 1); //1 = skip spam protection
      %client.play2D(rewardSound);
      $Tutorial::GoalsCompleted++;
      CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Print - Time: " @ %time, 3);
      CommandToClient(%client, 'CenterPrint', "", 0);
   }
}


//-----------------------------------
// Target Trigger
//  Activates when conditions are met
//-----------------------------------

datablock TriggerData(TutorialTargetsTrigger)
{
   tickPeriodMS = 50;
};

function TutorialTargetsTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialTargetsTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialTargetsTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if($TutorialCompleted)
      return;

   if(%obj.goalCompleted["Targets"])
      return;
      
   if(getSimTime() - %obj.lastTipTime < 300)
      return;
   %obj.lastTipTime = getSimTime();

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;
      
   for(%i=0;%i<%obj.dataBlock.maxTools;%i++)
   {
      if(%obj.tool[%i] == GunItem.getID())
      {
         %hasGun = 1;
         break;
      }
   }
   if((%image $= 0 || %image == BrickImage.getID()) && %hasGun)
   {
      %key = bindNameFix("useTools");
      CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip the Gun" ,0.5);
      return;
   }
   else if(%image !$= GunImage.getID() && %hasGun)
   {
      CommandToClient(%client, 'CenterPrint', "Use the \c3mouse wheel\c0 to scroll to the Gun" ,0.5);
      return;
   }
   else if(!%hasGun)
   {
      CommandToClient(%client, 'CenterPrint', "Run over the gun to pick it up" , 0.5);
      return;
   }
   else
   {
      %bind = bindNameFix("mouseFire");
      CommandToClient(%client, 'CenterPrint', "Prepare for Target Practice!\n\nAim at the targets and use "@%bind@" to fire" , 4);
      %client.play2d(AlarmSound);
      %trigger.targetSchedule = schedule(4000,0,"beginTargetPractice");
      %obj.goalCompleted["Targets"] = 1;
   }
}



//-----------------------------------
// Wand trigger
//  Tip trigger
//-----------------------------------

datablock TriggerData(TutorialWandTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialWandTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   %obj.canuseWand = 1;
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialWandTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   if(!$TutorialCompleted)
   {
      %obj.canuseWand = 0;
      if(%obj.getMountedImage(0).item $= "WandItem")
	   %obj.unmountImage(0);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialWandTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;
	
   if($TutorialCompleted)
      return;

   if(%obj.goalCompleted["Wand"])
      return;
      
   if(getSimTime() - %obj.lastTipTime < 300)
      return;
   %obj.lastTipTime = getSimTime();

   %key = bindNameFix("globalchat");
   if(%obj.getMountedImage(0) !$= WandImage.getID())
      centerPrint(%obj.client,"Open the chat box by pressing \c3"@%key@"\c0 and then type \c3/wand\c0 to equip the Wand.",2);
   else
      centerPrint(%obj.client,"Now use the wand to break the bricks. Unlike the hammer, the wand\ncan break bricks anywhere in a stack. \n\nHowever any bricks supported by it will also fall!",2);
   Parent::onTickTrigger(%this,%trigger);
}



//-----------------------------------
// Spraycan tip trigger
//  Hackish thing to detect when you spray bricks
//-----------------------------------

datablock TriggerData(TutorialSprayTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialSprayTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   %obj.canUseSpray = 1;
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialSprayTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   %client = %obj.client;
   if(%obj.getMountedImage(0).item $= "sprayCan")
   {      
	   %obj.unmountImage(0);
      fixArmReady(%obj);
      commandToClient(%client, 'setScrollMode', 3);
   }
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialSprayTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Spray"])
      return;

   if($TutorialCompleted)
      return;

   $DefaultMiniGame.setEnablePainting(1);

   %obj.lastTipTime = getSimTime();

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;
      
   %changedColors = 0;
   %changedColFX = 0;
   %changedShapeFX = 0;
   for(%i=1;%i<14;%i++)
   {
      %object = _TutorialSprayBrick@%i;
      if(isObject(%object))
      {
         if(%object.getColorID() !$= 4)
            %changedColors = 1;
         if(%object.getColorFXID() !$= 0)
            %changedColFX = 1;
         if(%object.getShapeFXID() !$= 0)
            %changedShapeFX = 1;
      }
   }
   
   %key = bindNameFix("useSprayCan");
   if(%obj.hasPainted)
   {
      CommandToClient(%client, 'ClearCenterPrint');

      //if we stop painting for a sec, count that as completing the goal
      %elapsedTime = getSimTime() - %obj.lastPaintTime;
      //echo("elapsed time " @ %elapsedTime);
      if(%elapsedTime > 2000)
      {
         %obj.CompleteTutorialGoal("Spray");
         openTutorialDoor("paintDoor");
      }
   }
   else
   {
      if(%image.item !$= "sprayCan")
      {
         if(!%obj.hasPainted)
            CommandToClient(%client, 'CenterPrint', "Press \c3" @ %key @ "\c0 to equip your Spray Can\nPress \c3"@%key@"\c0 again to switch paint columns\n\nUse the \c3mouse wheel\c0 to move up and down" ,4);
      }   
      else 
      {
         CommandToClient(%client, 'CenterPrint', "Make some \c3art", 3);
      }
   }
}



//-----------------------------------
// Treasure tip trigger
//  how to win the game
//-----------------------------------

datablock TriggerData(TutorialTreasureTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialTreasureTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialTreasureTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialTreasureTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Treasure"])
      return;

   if($TutorialCompleted)
      return;

   %obj.lastTipTime = getSimTime();

   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%client.numFoundTreasureChests >= 1)
   {
      %obj.schedule(2000, CompleteTutorial);
   }
   else
   {
      CommandToClient(%client, 'CenterPrint', "Treasure! Click the \c3chest!", 3);
   }
     
}

//-----------------------------------
// Jeep checkpoint trigger
//  Hackish thing to detect when you drive a vehicle in the box
//-----------------------------------

datablock TriggerData(TutorialJeepCheckTrigger)
{
   tickPeriodMS = 50;
};

function TutorialJeepCheckTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialJeepCheckTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialJeepCheckTrigger::onTickTrigger(%this,%trigger)
{
   Parent::onTickTrigger(%this,%trigger);
}

//-----------------------------------
// Drive tip trigger
//  Hackish thing to detect when you drive a vehicle correctly
//-----------------------------------

datablock TriggerData(TutorialDriveTipTrigger)
{
   tickPeriodMS = 50;
};

function TutorialDriveTipTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);  
}

function TutorialDriveTipTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialDriveTipTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
      
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;

   if(%obj.goalCompleted["Drive"])
      return;

   if($TutorialCompleted)
      return;

   if(getSimTime() - %obj.lastTipTime < 1000)
      return;

   %obj.lastTipTime = getSimTime();

   //need bind name, task name, goal name 
   %key = bindNameFix(%trigger.bindName);

   %image = %obj.getMountedImage(0);
   if(isObject(%image))
      %image = %image.getID();
   else
      %image = 0;

   if(!isObject(_TutorialVehicleBrick2.vehicle) && isObject(_TutorialVehicleBrick2))
   {
	_TutorialVehicleBrick2.setVehicle(JeepVehicle.getID());
	_TutorialVehicleBrick2.spawnVehicle();
   }
   else if(_TutorialVehicleBrick2.vehicle.dataBlock.getID() !$= JeepVehicle.getID() && isObject(_TutorialVehicleBrick2))
   {
	_TutorialVehicleBrick2.setVehicle(JeepVehicle.getID());
	_TutorialVehicleBrick2.spawnVehicle();
   }
   else if( _TutorialVehicleBrick2.vehicle.getMountNodeObject(0) !$= %obj && isObject(_TutorialVehicleBrick2) && !isObjectInsideObject(_TutorialVehicleBrick2.vehicle,JeepParkTrigger))
   {
	%keybind = bindNameFix("Jump");
	centerPrint(%client,"Press \c3"@%keybind@"\c0 to jump into the Jeep",3,3);
   }
   else if(_TutorialVehicleBrick2.vehicle.getMountNodeObject(0) $= %obj && _TutorialVehicleBrick2.vehicle.lastDrivingClient !$= "" && !isObjectInsideObject(_TutorialVehicleBrick2.vehicle,JeepParkTrigger) && isObject(_TutorialVehicleBrick2))
   {
	if(vectorDist(_TutorialVehicleBrick2.getPosition(),_TutorialVehicleBrick2.vehicle.getPosition()) < 5)
	{
	   if($pref::Input::UseStrafeSteering)
	   {
		   %wkey = strUpr(getField(movemap.getBinding(moveforward), 1));
         %akey = strUpr(getField(movemap.getBinding(moveleft), 1));
         %skey = strUpr(getField(movemap.getBinding(movebackward), 1));         
         %dkey = strUpr(getField(movemap.getBinding(moveright), 1));
		   centerPrint(%client,"Drive the Jeep by using \c3"@%wkey SPC %akey SPC %skey SPC %dkey@"\n\c0Complete the course and park the jeep",5,3);
	   }
	   else
	   {
         %wkey = strUpr(getField(movemap.getBinding(moveforward), 1));
         %skey = strUpr(getField(movemap.getBinding(movebackward), 1));
         centerPrint(%client,"Accelerate the Jeep by using \c3"@%wkey@"\c0 and brake using \c3"@%skey@"\n\c0You can steer using the \c3mouse\c0\nComplete the course and park the jeep",5,3);
	   }
	}
	else
	   centerPrint(%client,"Complete the course and park the jeep",5,3);
   }
   else if(_TutorialVehicleBrick2.vehicle.getMountNodeObject(0) !$= %obj && !isObjectInsideObject(_TutorialVehicleBrick2.vehicle,JeepParkTrigger) && isObject(_TutorialVehicleBrick2) && isPositionInsideObject(_TutorialVehicleBrick2.vehicle.getPosition(),JeepParkTrigger))
   {
	centerPrint(%client,"Ooops - You'll have to do a better parking than that!",3,3);
   }
   else if(_TutorialVehicleBrick2.vehicle.getMountNodeObject(0) $= %obj && isObjectInsideObject(_TutorialVehicleBrick2.vehicle,JeepParkTrigger) && isObject(_TutorialVehicleBrick2))
   {
	%keybind = bindNameFix("Jet");
	centerPrint(%client,"You can now get out of the Jeep by pressing \c3"@%keybind,3,3);
   }
   else
   {
	openTutorialDoor(5);
      %obj.goalCompleted["Drive"] = true;
      %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
      
      %obj.emote(winStarProjectile, 1); //1 = skip spam protection
      %client.play2D(rewardSound);
      $Tutorial::GoalsCompleted++;
      CommandToClient(%client, 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Drive - Time: " @ %time, 3);
      CommandToClient(%client, 'CenterPrint', "", 0);
   }
}

function isObjectInsideObject(%obj1,%obj2)
{
   if(!isObject(%obj1) || !isObject(%obj2))
      return;
      
   %obj1WB = %obj1.getWorldBox();
   %obj1MinX = getWord(%obj1WB,0);
   %obj1MinY = getWord(%obj1WB,1);
   %obj1MinZ = getWord(%obj1WB,2);
   %obj1MaxX = getWord(%obj1WB,3);
   %obj1MaxY = getWord(%obj1WB,4);
   %obj1MaxZ = getWord(%obj1WB,5);
   %obj2WB = %obj2.getWorldBox();
   %obj2MinX = getWord(%obj2WB,0);
   %obj2MinY = getWord(%obj2WB,1);
   %obj2MinZ = getWord(%obj2WB,2);
   %obj2MaxX = getWord(%obj2WB,3);
   %obj2MaxY = getWord(%obj2WB,4);
   %obj2MaxZ = getWord(%obj2WB,5);
   if(%obj1MinX >= %obj2MinX && %obj1MaxX <= %obj2MaxX && %obj1MinY >= %obj2MinY && %obj1MaxY <= %obj2MaxY && %obj1MinZ >= %obj2MinZ && %obj1MaxZ <= %obj2MaxZ)
      return true;
   else
      return false;
}

function isPositionInsideObject(%pos,%obj1)
{
   %WB = %obj1.getWorldBox();
   %XMin = getWord(%WB,0);
   %YMin = getWord(%WB,1);
   %ZMin = getWord(%WB,2);
   %XMax = getWord(%WB,3);
   %YMax = getWord(%WB,4);
   %ZMax = getWord(%WB,5);
   %PosX = getWord(%pos,0);
   %PosY = getWord(%pos,1);
   %PosZ = getWord(%pos,2);
   if(%PosX > %XMin && %PosX < %XMax && %PosY > %YMin && %PosY < %YMax && %PosZ > %ZMin && %PosZ < %ZMax)
      return true;
   else
      return false;
}

//-----------------------------------
// full win trigger
//  for detecting when you complete the entire tutorial
//-----------------------------------
datablock TriggerData(TutorialFullWinTrigger)
{
   tickPeriodMS = 100;
};

function TutorialFullWinTrigger::onEnterTrigger(%this,%trigger,%obj)
{
   Parent::onEnterTrigger(%this,%trigger,%obj);
   
   if(!(%obj.getType() & $TypeMasks::PlayerObjectType))
      return;
   %client = %obj.client;
   if(!isObject(%client))
      return;
      
   if($TutorialCompleted)
      return;
      
   %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
   
   %obj.emote(winStarProjectile, 1); //1 = skip spam protection
   %client.play2D(rewardSound);
   clearBottomPrint(%client);
   clearCenterPrint(%client);

   //show our stats on the completed dialog
   %prefix = "<font:arial:18><just:right>";
   %time = getTimeString((getSimTime() - %obj.spawnTime) / 1000);
   TutorialCompletedGui_Time.setText(%prefix @ %time);
   TutorialCompletedGui_Rooms.setText(%prefix @ $Tutorial::GoalsCompleted @ "/18");
   TutorialCompletedGui_Targets.setText(%prefix @ $Tutorial::TotalTargetsHit@ "/" @ $Tutorial::TotalTargetsLaunched);
   %percent = mCeil(($Tutorial::TotalTargetsHit/$Tutorial::TotalShotsFired)*100);
   TutorialCompletedGui_Accuracy.setText(%prefix @  %percent @ "%");

   canvas.pushdialog(tutorialCompletedGui);
   

   %client.player.setDataBlock(PlayerStandardArmor);

   %client.player.goalCompleted["Wand"] = 1;

   //Var to enable special hat or something?
   $Pref::Player::TutorialCompleted = 1;
}

function stayAndBuild()
{
   $TutorialCompleted = 1;

   TutorialSpawn.setTransform("-83.3524 -91.2663 95.958 0 0 1 0");
   TutorialSpawn.radius = 5;
}

function TutorialFullWinTrigger::onLeaveTrigger(%this,%trigger,%obj)
{
   Parent::onLeaveTrigger(%this,%trigger,%obj);
}

function TutorialFullWinTrigger::onTickTrigger(%this,%trigger, %obj)
{  
   Parent::onTickTrigger(%this,%trigger);
}



//get bind names
function bindNameFix(%bindName)
{
   %device = getField(movemap.getBinding(%bindName), 0);
   %device = getSubStr(%device, 0, 5);
   %key = getField(movemap.getBinding(%bindName), 1);

   if(%device $= "mouse")
   {
      switch$(%key)
      {
         case "button0":
            %key = "left mouse button";
         case "button1":
            %key = "right mouse button";
         case "button2":
            %key = "middle mouse button";
         default:
            %key = "mouse " @ %key;
      }
   }
   else
   {
      switch$(%key)
      {
         case "space":
            %key = "spacebar";
         case "lshift":
            %key = "left shift";
         case "rshift":
            %key = "right shift";
         case "lalt":
            %key = "left alt";
         case "raltt":
            %key = "right alt";
      }
      
      if(strlen(%key) == 1)
         %key = strUpr(%key);
   }

   return %key;
}


//player datablocks to disable jets and jumping and such
datablock PlayerData(PlayerTutorialNoMove : PlayerStandardArmor)
{  
   airControl = 0;

	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

	uiName = "";
	showEnergyBar = false;
   jumpForce = 0;
   runforce = 0;
};

datablock PlayerData(PlayerTutorialNoJumpNoJet: PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

	uiName = "";
	showEnergyBar = false;
   jumpForce = 0;
};

datablock PlayerData(PlayerTutorialNoJet : PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

	uiName = "";
	showEnergyBar = false;
};

datablock PlayerData(PlayerTutorialTarget : PlayerStandardArmor)
{
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;

   maxForwardSpeed = 5;

	uiName = "";
	showEnergyBar = false;
};

//-----------------------------------
// Target Shooting
//  Reads targetSetup.txt - Syntax:
//    first number = row (1-3)
//    second number = speed (1-5)
//    timeout: x = time between each target launch = x
//    line break: 1000ms timeout
//-----------------------------------

//datablock StaticShapeData(TutorialTarget)
//{
//	shapeFile = "./target.dts";
//};
//datablock StaticShapeData(TutorialTargetHit)
//{
//	shapeFile = "./targetHit.dts";
//};
//datablock StaticShapeData(TutorialTargetM)
//{
//	shapeFile = "./targetM.dts";
//};
//datablock StaticShapeData(TutorialTargetMHit)
//{
//	shapeFile = "./targetMHit.dts";
//};

function tutorialPlaceBrick(%name,%type,%position,%rotation,%color)
{
   switch(%rotation)
   {
      case 0:
         %angularRotation = "1 0 0 0";
      case 1:
         %angularRotation = "0 0 1" SPC $piOver2;
      case 2:
         %angularRotation = "0 0 1" SPC $pi;
      case 3:
         %angularRotation = "0 0 -1" SPC $piOver2;
   }
   
   %brick = new fxDTSBrick(%name)
   {
      dataBlock = %type;
      position = %position;
      angleID = %rotation;
      colorID = %color;
      isPlanted = true;
      isBaseplate = true;
      
      colorFXID = "";
      shapeFXID = "";
      printID = "";
   };
   %brick.setTransform(%position SPC %angularRotation);
   %brick.setTrusted(1);
   %brick.plant();
   
   clientgroup.getobject(0).brickGroup.add(%brick);
   return %brick;
}

function beginTargetPractice()
{
   %file = new FileObject();
   if(%file.openForRead("Add-Ons/Map_Tutorial/targetSetup.txt"))
   {
      $Tutorial::TargetFile = %file;
      $Tutorial::ScheduleTime = 1000;
      $Tutorial::TotalTargetsLaunched = 0;
      $Tutorial::TotalTargetsHit = 0;
      $Tutorial::TotalShotsFired = 0;
      readTargetLine();
   }
   else
   {
      error("TUTORIAL ERROR: Target File not Found.");
      %file.close();
      %file.delete();
   }
}

function readTargetLine()
{
   %file = $Tutorial::TargetFile;
   if(!isObject(%file))
      return;
   if(%file.isEOF())
   {
      stopTargetPractice(1);
      return;
   }
   %line = %file.readLine();
   if(%line $= "")
      $Tutorial::TargetSchedule = schedule(1000,0,"readTargetLine");
   else
   {
      %targetRow = getWord(%line,0);
      %targetSpeed = getWord(%line,1);
	%targetType = getWord(%line,2);
      
      if(%targetRow $= "timeout:")
      {
         $Tutorial::ScheduleTime = %targetSpeed;
         $Tutorial::TargetSchedule = schedule($Tutorial::ScheduleTime,0,"readTargetLine");
         return;
      }
      
      launchTarget(%targetRow,%targetSpeed,%targetType);
         
      $Tutorial::TotalTargetsLaunched++;
      $Tutorial::TargetSchedule = schedule($Tutorial::ScheduleTime,0,"readTargetLine");
   }
}

function stopTargetPractice(%finish)
{
   if(%finish)
      checkForEnd();
      
   cancel($Tutorial::TargetSchedule);
   if(isObject($Tutorial::TargetFile))
      $Tutorial::TargetFile.delete();
}

function checkForEnd()
{
   if(TutorialTargets.getCount() $= 0)
   {
      %percent = mCeil(($Tutorial::TotalTargetsHit/$Tutorial::TotalShotsFired)*100);
      
      %time = getTimeString((getSimTime() - clientgroup.getobject(0).player.spawnTime) / 1000);
      
      clientgroup.getobject(0).player.emote(winStarProjectile, 1); //1 = skip spam protection
      clientgroup.getobject(0).play2D(rewardSound);
      $Tutorial::GoalsCompleted++;
      CommandToClient(clientgroup.getobject(0), 'BottomPrint', "<bitmap:base/client/ui/CI/trophy>\c3 Goal Completed! - Shooting - Time: " @ %time@"\n("@$Tutorial::TotalTargetsHit@"/"@$Tutorial::TotalTargetsLaunched@" targets hit with "@%percent@"% accuracy)",8,3);
      CommandToClient(clientgroup.getobject(0), 'CenterPrint', "", 0);
	openTutorialDoor(4);
   }
   else
      schedule(1000,0,"checkForEnd");
}

function launchTarget(%row,%speed,%type)
{
   if(!isObject(TutorialTargets))
      new SimGroup(TutorialTargets);
      
   if(%speed $= "")
      %speed = 1;

   if(strPos(%type,"m") >= 0)
	%data = TutorialTargetM;
   else
	%data = TutorialTarget;

   %target = new StaticShape()
   {
      dataBlock = %data;
      speed = %speed;
   };
   TutorialTargets.add(%target);
   
   if(%type $= "m4")
	%target.setSkinName("m3");
   else if(%type $= "m3")
	%target.setSkinName("m2");
   else if(%type $= "m2")
	%target.setSkinName("m1");
   else if(%type $= "m1")
	%target.setSkinName("base");

   if(%row == 1)
      %target.setTransform("-44.8628 -71.371 94.4225" SPC eulerToQuat("0 0 -90"));
   else if(%row == 2)
      %target.setTransform("-44.8628 -64.8711 94.4225" SPC eulerToQuat("0 0 -90"));
   else if(%row == 3)
      %target.setTransform("-44.8628 -58.8758 94.4225" SPC eulerToQuat("0 0 -90"));
      
   scrollTarget(%target);
}

function scrollTarget(%target)
{
   if(!isObject(%target))
      return;
      
   %pos = %target.getPosition();
   if(getWord(%pos,0) > -32)
   {
      %target.delete();
      return;
   }
   
   switch(%target.speed)
   {
      case 1:
      %moveAmmount = 0.06;
      %scheduleAmmount = 30;
      case 2:
      %moveAmmount = 0.08;
      %scheduleAmmount = 25;
      case 3:
      %moveAmmount = 0.09;
      %scheduleAmmount = 25;
      case 4:
      %moveAmmount = 0.1;
      %scheduleAmmount = 20;
      case 5:
      %moveAmmount = 0.17;
      %scheduleAmmount = 20;
   }
   
   %newPos = vectorAdd(%pos,%moveAmmount SPC "0 0");
   %target.setTransform(%newpos SPC rotFromTransform(%target.getPosition()));
   schedule(%scheduleAmmount,0,"scrollTarget",%target);
}




package TutorialPackage
{
   //this function is called when save.bls is finished loading
   function GameModeInitialResetCheck()
   {
      Parent::GameModeInitialResetCheck();

      //load the bricks that we want to be able to modify
      schedule(0, 0, serverDirectSaveFileLoad, "Add-Ons/GameMode_Tutorial/userBricks.bls", 3, "", 0, 1);

      //create triggers
      if(isFile("Add-Ons/GameMode_Tutorial/createTriggers.cs"))
      {
         exec("Add-Ons/GameMode_Tutorial/createTriggers.cs");
         MissionCleanup.add(TutorialGroup);
      }

      //loop through all datablocks, clear out uinames for all bricks except 2x4
      %group = DataBlockGroup;
      %count = DataBlockGroup.getCount();
      for(%i = 0; %i < %count; %i++)
      {
         %data = %group.getObject(%i);
         if(%data.getClassName() !$= "fxDTSBrickData")
            continue;

         if(%data.uiName !$= "2x4")
            %data.uiName = "";
      }

      //tell client to rebuild brick selector
      commandToAll('BSD_LoadBricks');

      //don't show wireframe on invisible bricks
      wrenchImage.showBricks = false;
      hammerImage.showBricks = false;
      brickImage.showBricks  = false;

      //may have variables from a previous session, delete them now
      deleteVariables("$Tutorial::*");
   }

   function brick2x4Data::onTrustCheckFinished(%data, %brick)
   {
      %client = %brick.client;
      if(!isObject(%client))
         %client = %brick.getGroup().client;
      if(!isObject(%client))
         %client = clientGroup.getObject(0);
      if(!isObject(%client))
         return;

      %player = %client.player;
      if(!isObject(%player))
         return;

      //hack: don't do this while loading
      if(isObject($Server_LoadFileObj))
         return;

      //check if we've planted onto one of the build goals
      if(!%player.goalCompleted["Build1"])
      {
         %count = %brick.getNumDownBricks();
         for(%i = 0; %i < %count; %i++)
         {
            %downBrick = %brick.getDownBrick(%i);
            //echo("downbrick " @ %downbrick);
            if(%downBrick.getName() $= "_build1Platform")
            {  
               if(isObject(%client))
               {
                  %player.completeTutorialGoal("Build1");
                  openTutorialDoor("build1Door");
                  return;
               }     
            }
         }
         %brick.killBrick();
      }

      if(!%player.goalCompleted["Build2"])
      {
         %pos = %brick.getPosition();
         %x = getWord(%pos, 0);
         %y = getWord(%pos, 1);
         %z = getWord(%pos, 2);

         if(mabs(%x -  -1.5) < 0.01 &&
            mabs(%y - -10.5) < 0.01 && 
            mabs(%z -  17.5) < 0.01 )
         {
            %player.completeTutorialGoal("Build2");
            openTutorialDoor("build2Door");
         }
         else
         {
           %brick.killBrick(); 
         }
      }
   }

   function paintProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
   {
      Parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);

      %client = %obj.client;
      %player = %client.player;
      %player.hasPainted = true;
      %player.lastPaintTime = getSimTime();
   }

   function hammerImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
   {
      //don't allow player to break the wrench brick
      if(%hitObj.getName() $= "_TutorialWrenchBrick" || %hitObj.getId() == $Tutorial::WrenchBrick)
         return;
      
      Parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
   }

   //don't show "You cannot modify public bricks" message
   function SimGroup::getTrustFailureMessage(%group)
   {
      %msg = "";
      if(%group.bl_id != 888888)   
         %msg = Parent::getTrustFailureMessage(%group);
      
      return %msg;
   }
};
activatePackage(TutorialPackage);


