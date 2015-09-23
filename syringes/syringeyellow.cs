//########## Syringe Red (Adrenaline)

//### Effects

datablock ParticleData(gc_SyringeAdrenalineParticle)
{
  dragCoefficient = 2;
  gravityCoefficient = -0.2;
  inheritedVelFactor = 0;
  constantAcceleration = 0;
  lifetimeMS = 1000;
  lifetimeVarianceMS = 500;
  textureName = "./cross";
  spinSpeed = 0;
  spinRandomMin = -1;
  spinRandomMax = 1;
  colors[0] = "1 1 0 0";
  colors[1] = "1 1 0 1";
  colors[2] = "1 1 0 0";
  sizes[0] = 0.2;
  sizes[1] = 0.4;
  sizes[2] = 0.2;
  times[0] = 0;
  times[1] = 0.5;
  times[2] = 1;
  useInvAlpha = false;
};

datablock ParticleEmitterData(gc_SyringeAdrenalineEmitter)
{
  uiName = "";
  ejectionPeriodMS = 5;
  periodVarianceMS = 0;
  ejectionVelocity = 0;
  velocityVariance = 0;
  ejectionOffset = 1;
  thetaMin = 0;
  thetaMax = 180;
  phiReferenceVel = 0;
  phiVariance = 360;
  overrideAdvance = false;
  particles = "gc_SyringeAdrenalineParticle";
};

datablock ExplosionData(gc_SyringeAdrenalineExplosion)
{
  lifeTimeMS = 200;
  emitter[0] = gc_SyringeAdrenalineEmitter;
  faceViewer = true;
  explosionScale = "1 1 1";
};

datablock ProjectileData(gc_SyringeAdrenalineEffect)
{
  uiName = "";
  explosion = gc_SyringeAdrenalineExplosion;
  lifetime = 1;
  fadeDelay = 1;
  explodeOnDeath = true;
};

datablock PlayerData(gc_AdrenalinePlayer : PlayerOTSNoJet)
{
  uiName = "";
  runForce = 8000;
  maxForwardSpeed = 21;
  maxBackwardSpeed = 12;
  maxSideSpeed = 18;
  maxForwardCrouchSpeed = 9;
  maxBackwardCrouchSpeed = 6;
  maxSideCrouchSpeed = 6;
  jumpForce = 1500;
  airControl = 1;
  canJet = 0;
};

//### Item

datablock ItemData(gc_SyringeAdrenalineItem)
{
  uiName = "Syringe Adrenaline";
  iconName = "./icon_syringeyellow";
  image = gc_SyringeAdrenalineImage;
  category = Weapon;
  className = Weapon;
  shapeFile = "./syringeyellow.dts";
  mass = 1;
  density = 0.2;
  elasticity = 0;
  friction = 0.6;
  emap = true;
  doColorShift = true;
  colorShiftColor = "1 1 1 1";
  canDrop = true;
  gc_syringe = 1;
};

//### Item Image

datablock shapeBaseImageData(gc_SyringeAdrenalineImage)
{
  shapeFile = "./syringeyellow.dts";
  emap = true;
  correctMuzzleVector = false;
  className = "WeaponImage";
  item = gc_SyringeAdrenalineItem;
  ammo = "";
  projectile = "";
  projectileType = Projectile;
  melee = false;
  doReaction = false;
  armReady = true;
  doColorShift = true;
  colorShiftColor = "1 1 1 1";

  stateName[0] = "Activate";
  stateTimeoutValue[0] = 0.2;
  stateTransitionOnTimeout[0] = "Ready";
  stateSound[0] = weaponSwitchSound;

  stateName[1] = "Ready";
  stateTransitionOnTriggerDown[1] = "Charge";
  stateAllowImageChange[1] = true;

  stateName[2] = "Charge";
  stateTransitionOnTimeout[2] = "Armed";
  stateTimeoutValue[2] = 0.2;
  stateWaitForTimeout[2] = false;
  stateTransitionOnTriggerUp[2] = "AbortCharge";
  stateScript[2] = "onCharge";
  stateAllowImageChange[2] = false;

  stateName[3] = "AbortCharge";
  stateTransitionOnTimeout[3] = "Ready";
  stateTimeoutValue[3] = 0.3;
  stateWaitForTimeout[3] = true;
  stateScript[3] = "onAbortCharge";
  stateAllowImageChange[3] = false;

  stateName[4] = "Armed";
  stateTransitionOnTriggerUp[4] = "Fire";
  stateAllowImageChange[4] = false;

  stateName[5] = "Fire";
  stateTransitionOnTimeout[5] = "Ready";
  stateTimeoutValue[5] = 0.2;
  stateFire[5] = true;
  stateScript[5] = "onFire";
  stateWaitForTimeout[5] = true;
  stateAllowImageChange[5] = false;
};

function gc_SyringeAdrenalineImage::onMount(%this,%obj,%slot) { bottomPrint(%obj.client,"<color:ffff00>[LMB]<color:00ff00> use on others <color:ffff00>[RMB]<color:00ff00> use on self",5); }
function gc_SyringeAdrenalineImage::onCharge(%this,%obj,%slot) { %obj.playThread(2,spearReady); }
function gc_SyringeAdrenalineImage::onAbortCharge(%this,%obj,%slot) { %obj.playthread(2,root); }

function gc_SyringeAdrenalineImage::onFire(%this,%obj,%slot)
{
  %obj.playThread(2,spearThrow);
  %raycast = containerRayCast(%obj.getEyePoint(),vectorAdd(%obj.getEyePoint(),vectorScale(%obj.getEyeVector(),3)),$TypeMasks::PlayerObjectType,%obj);
  %col = firstWord(%raycast);
  if(isObject(%col) &&  %col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
  {
      %col.spawnExplosion(gc_SyringeAdrenalineEffect,"1 1 1");
      if(%col.getDataBlock().getName() $= "gc_AdrenalinePlayer")
      {
        cancel(%col.ResetDatablockLater);
        %col.ResetDataBlockLater = %col.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%col.lastDatablock);
        %currSlot = %obj.currTool;
        %obj.tool[%currSlot] = 0;
        %obj.weaponCount--;
        messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
        serverCmdUnUseTool(%obj.client);
        return;
      }
      if(%col.getDataBlock().getName() $= "gc_SedativePlayer")
      {
        cancel(%col.ResetDatablockLater);
        %col.setDatablock(%col.lastDatablock);
        %currSlot = %obj.currTool;
        %obj.tool[%currSlot] = 0;
        %obj.weaponCount--;
        messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
        serverCmdUnUseTool(%obj.client);
        return;
      }
      %col.lastDatablock = %col.getDataBlock();
      %col.setDataBlock("gc_AdrenalinePlayer");
      %col.ResetDataBlockLater = %col.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%col.lastDatablock);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}

function gc_SyringeAdrenalineImage::onSelfUse(%this,%obj,%slot)
{
  if(%obj.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
  {
      %obj.playThread(2,spearThrow);
      %obj.spawnExplosion(gc_SyringeAdrenalineEffect,"1 1 1");
      if(%obj.getDataBlock().getName() $= "gc_AdrenalinePlayer")
      {
        cancel(%obj.ResetDatablockLater);
        %obj.ResetDataBlockLater = %obj.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%obj.lastDatablock);
        %currSlot = %obj.currTool;
        %obj.tool[%currSlot] = 0;
        %obj.weaponCount--;
        messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
        serverCmdUnUseTool(%obj.client);
        return;
      }
      if(%obj.getDataBlock().getName() $= "gc_SedativePlayer")
      {
        cancel(%obj.ResetDatablockLater);
        %obj.setDatablock(%obj.lastDatablock);
        %currSlot = %obj.currTool;
        %obj.tool[%currSlot] = 0;
        %obj.weaponCount--;
        messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
        serverCmdUnUseTool(%obj.client);
        return;
      }
      %obj.lastDatablock = %obj.getDataBlock();
      %obj.setDataBlock("gc_AdrenalinePlayer");
      %obj.ResetDataBlockLater = %obj.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%obj.lastDatablock);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}
