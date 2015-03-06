//--- OBJECT WRITE BEGIN ---
new SimGroup(TutorialGroup) {

   new Trigger(Trigger_JumpWin) {
      position = "-1.65376 31.1151 15.6238";
      rotation = "0 0 1 89.9544";
      scale = "14.345 6.8263 4.16024";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Jump";
   };
   new Trigger(Trigger_MoveWin) {
      position = "7.31149 31.1873 14.8761";
      rotation = "1 0 0 0";
      scale = "14.1169 14.1114 4.16024";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Move";
   };
   new Trigger(Trigger_DuckWin) {
      position = "5.9513 11.1785 15.3618";
      rotation = "0 0 1 90.5273";
      scale = "6.42236 1.41644 4.16024";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Duck";
   };
   new Trigger(Trigger_BuildWin) {
      position = "-25.6205 -9.48533 21.9823";
      rotation = "0 0 1 179.909";
      scale = "0.806694 2.93881 5.78381";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Build";
   };
   new Trigger(Trigger_JumpTip) {
      position = "6.07078 30.7386 15.7813";
      rotation = "0 0 1 89.9544";
      scale = "14.0926 4.00669 4.16024";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "jump";
         goalName = "Jump";
         bindName = "jump";
   };
   new Trigger(Trigger_BreakWin) {
      position = "-59.0709 41.5361 15.682";
      rotation = "1 0 0 0";
      scale = "1.82585 3.03382 6.17545";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Break";
   };
   new Trigger(Trigger_JetWin) {
      position = "-35.3981 -1.25503 35.1381";
      rotation = "1 0 0 0";
      scale = "3.89038 4.04094 5.76116";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Jet";
   };
   new Trigger(Trigger_LightStart) {
      position = "-36.1501 9.54121 28.3605";
      rotation = "1 0 0 0";
      scale = "5.15247 3.16554 5.76116";
      dataBlock = "TutorialLightStartTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Jet";
   };
   new Trigger(Trigger_CrouchTip) {
      position = "2.50936 15.5414 15.3618";
      rotation = "0 0 1 90.5273";
      scale = "14.2508 11.6118 4.16024";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "crouch";
         goalName = "Duck";
         bindName = "crouch";
   };
   new Trigger(Trigger_JetTip) {
      position = "-35.4369 -5.93602 16.564";
      rotation = "1 0 0 0";
      scale = "4.03705 4.0261 3.89381";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "jet";
         goalName = "Jet";
         bindName = "jet";
   };
   new Trigger(Trigger_LightTip) {
      position = "-25.7262 14.1251 16.0412";
      rotation = "0 0 1 89.9544";
      scale = "13.3096 14.9695 11.0142";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "turn on your light";
         goalName = "Light";
         bindName = "uselight";
   };
   new Trigger(Trigger_LightWin) {
      position = "-35.5788 16.4669 16.0008";
      rotation = "0 0 1 89.9544";
      scale = "0.904398 1.85337 5.36993";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Light";
   };
   new Trigger(Trigger_LookTip) {
      position = "13.4193 25.1114 24.9568";
      rotation = "1 0 0 0";
      scale = "2.07324 2.25445 4.16024";
      dataBlock = "TutorialLookTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Marker(TutorialLookTarget) {
      position = "7.90821 24.0576 26.936";
      rotation = "1 0 0 0";
      scale = "1 1 1";
      seqNum = "14";
      type = "Normal";
      msToNext = "1000";
      smoothingType = "Spline";
   };
   new Trigger(Trigger_BrickTip) {
      position = "7.41411 15.1166 16.1921";
      rotation = "1 0 0 0";
      scale = "14.0844 14.1098 4.16024";
      dataBlock = "TutorialBrickTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get bricks";
         goalName = "Brick";
         bindName = "openBSD";
   };
   new Trigger(Trigger_BreakTip) {
      position = "-55.9682 47.5275 15.8825";
      rotation = "1 0 0 0";
      scale = "14.3595 14.0215 4.16024";
      dataBlock = "TutorialBreakTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get bricks";
         goalName = "Brick";
         bindName = "openBSD";
   };
   new Trigger(Trigger_BuildTip1) {
      position = "21.7387 -0.992064 15.9328";
      rotation = "0 0 1 90.5273";
      scale = "14.2112 14.0574 4.16024";
      dataBlock = "TutorialBuildTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get bricks";
         goalName = "Brick";
         bindName = "openBSD";
   };
   new Trigger(Trigger_BuildTip2) {
      position = "5.74887 -0.021471 15.9328";
      rotation = "0 0 1 90.5273";
      scale = "14.2112 14.0574 4.16024";
      dataBlock = "TutorialBuildTipTriggerB";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get bricks";
         goalName = "Brick";
         bindName = "openBSD";
   };
   new Trigger(Trigger_BuildTip3) {
      position = "-10.4684 -0.834064 15.9328";
      rotation = "0 0 1 90.5273";
      scale = "14.2112 14.0574 4.16024";
      dataBlock = "TutorialBuildTipTriggerC";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get bricks";
         goalName = "Brick";
         bindName = "openBSD";
   };
   new Trigger(Trigger_WrenchTip) {
      position = "-72.6509 46.9642 15.8774";
      rotation = "1 0 0 0";
      scale = "14.3543 13.9728 4.38031";
      dataBlock = "TutorialWrenchTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger() {
      position = "-75.6504 -83.1011 94.3914";
      rotation = "1 0 0 0";
      scale = "14.7607 15.1736 11.8968";
      dataBlock = "TutorialFullWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_RideTip) {
      position = "-26.2545 23.73 16.0426";
      rotation = "0 0 1 90.5273";
      scale = "6.97061 14.2366 10.2724";
      dataBlock = "TutorialRideTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_DivingTipJump) {
      position = "-41.2081 -88.3371 85.9011";
      rotation = "1 0 0 0";
      scale = "5.40918 6.15538 9.81239";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "get out of the water";
         goalName = "Diving";
         bindName = "jump";
   };
   new Trigger(Trigger_Drive) {
      position = "-75.5551 -53.9699 92.9465";
      rotation = "1 0 0 0";
      scale = "29.0719 29.1848 13.0931";
      dataBlock = "TutorialDriveTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(JeepParkTrigger) {
      position = "-74.8967 -55.298 93.1667";
      rotation = "1 0 0 0";
      scale = "9.2918 9.42666 10.9552";
      dataBlock = "TutorialJeepCheckTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_Secrets) {
      position = "-46.0974 -128.876 88.7886";
      rotation = "1 0 0 0";
      scale = "29.4237 14.8988 16.9588";
      dataBlock = "TutorialSecretTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_Wand) {
      position = "-90.4136 -68.8 93.645";
      rotation = "1 0 0 0";
      scale = "14.4554 14.2719 12.0617";
      dataBlock = "TutorialWandTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_SecretsWin) {
      position = "-41.9428 -132.531 94.2462";
      rotation = "1 0 0 0";
      scale = "6.88423 6.79305 1.7268";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Secrets";
   };
   new Trigger(Trigger_Spray) {
      position = "-73.1683 31.5843 16.134";
      rotation = "1 0 0 0";
      scale = "15.3373 15.0089 11.418";
      dataBlock = "TutorialSprayTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_Treasure) {
      position = "-89.6301 31.5735 16.134";
      rotation = "1 0 0 0";
      scale = "15.3373 15.0089 11.418";
      dataBlock = "TutorialTreasureTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_Water) {
      position = "-26.2751 40.0003 14.5231";
      rotation = "0 0 1 90.5273";
      scale = "16.418 14.2038 1";
      dataBlock = "TutorialWaterTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_RideWin) {
      position = "-26.1065 41.7742 16.2685";
      rotation = "0 0 1 90.5273";
      scale = "1.64546 14.3838 10.7277";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Ride";
   };
   new Trigger(Trigger_DismountTip) {
      position = "-26.6897 46.2859 16.0108";
      rotation = "0 0 1 90.5273";
      scale = "5.56514 14.0816 9.53586";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "Dismount";
         goalName = "Dismount";
         bindName = "Jet";
   };
   new Trigger(Trigger_PrintTip) {
      position = "-30.535 -98.9356 94.1082";
      rotation = "1 0 0 0";
      scale = "14.1212 14.3815 9.00685";
      dataBlock = "TutorialPrintTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
   };
   new Trigger(Trigger_DivingTipCrouch) {
      position = "-26.1633 -88.8024 92.8493";
      rotation = "1 0 0 0";
      scale = "5.40918 5.42935 4.19852";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "dive underwater";
         goalName = "Diving";
         bindName = "Crouch";
   };
   new Trigger(Trigger_Targets) {
      position = "-45.8622 -53.3533 94.2681";
      rotation = "1 0 0 0";
      scale = "15.2422 30.1189 11.418";
      dataBlock = "TutorialTargetsTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         targetSchedule = "1562";
   };
   new Trigger(Trigger_DivingWin) {
      position = "-41.4125 -88.5488 97.4207";
      rotation = "1 0 0 0";
      scale = "5.65512 5.75327 1";
      dataBlock = "TutorialWinTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         goalName = "Diving";
   };
   new Trigger(Trigger_DivingTipJet) {
      position = "-34.1829 -87.507 84.2315";
      rotation = "1 0 0 0";
      scale = "7.60415 7.34955 7.13723";
      dataBlock = "TutorialTipTrigger";
      polyhedron = "0.0000000 0.0000000 0.0000000 1.0000000 0.0000000 0.0000000 0.0000000 -1.0000000 0.0000000 0.0000000 0.0000000 1.0000000";
         taskName = "swim faster";
         goalName = "Diving";
         bindName = "jet";
         allowJet = "1";
   };
};
//--- OBJECT WRITE END ---
