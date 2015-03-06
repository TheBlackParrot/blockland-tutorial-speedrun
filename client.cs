exec("add-ons/GameMode_Tutorial/tutorialCompletedGui.gui");

function clientCmdTutorialCompleted()
{
   canvas.pushDialog(TutorialCompletedGui);

   if($Pref::Player::TutorialCompleted == 0)
      steamGetAchievement("ACH_EDUCATED", "steamGetAchievement");

   $Pref::Player::TutorialCompleted = 1;   
}