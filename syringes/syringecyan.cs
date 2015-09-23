//########## Syringe Cyan (Sedative)

//### Effects

datablock ParticleData(gc_SyringeSedativeParticle)
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
  colors[0] = "0 0.75 1 0";
  colors[1] = "0 0.75 1 1";
  colors[2] = "0 0.75 1 0";
  sizes[0] = 0.2;
  sizes[1] = 0.4;
  sizes[2] = 0.2;
  times[0] = 0;
  times[1] = 0.5;
  times[2] = 1;
  useInvAlpha = false;
};

datablock ParticleEmitterData(gc_SyringeSedativeEmitter)
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
  particles = "gc_SyringeSedativeParticle";
};

datablock ExplosionData(gc_SyringeSedativeExplosion)
{
  lifeTimeMS = 200;
  emitter[0] = gc_SyringeSedativeEmitter;
  faceViewer = true;
  explosionScale = "1 1 1";
};

datablock ProjectileData(gc_SyringeSedativeEffect)
{
  uiName = "";
  explosion = gc_SyringeSedativeExplosion;
  lifetime = 1;
  fadeDelay = 1;
  explodeOnDeath = true;
};

datablock PlayerData(gc_SedativePlayer : PlayerOTSNoJet)
{
  uiName = "";
  maxForwardSpeed = 0.5;
  maxBackwardSpeed = 0.5;
  maxSideSpeed = 0.5;
  maxForwardCrouchSpeed = 0.5;
  maxBackwardCrouchSpeed = 0.5;
  maxSideCrouchSpeed = 0.5;
  canJet = 0;
  jumpForce = 0;
  airControl = 0;
};

//### Item

datablock ItemData(gc_SyringeSedativeItem)
{
  uiName = "Syringe Sedative";
  iconName = "./icon_syringecyan";
  image = gc_SyringeSedativeImage;
  category = Weapon;
  className = Weapon;
  shapeFile = "./syringecyan.dts";
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

datablock shapeBaseImageData(gc_SyringeSedativeImage)
{
  shapeFile = "./syringecyan.dts";
  emap = true;
  correctMuzzleVector = false;
  className = "WeaponImage";
  item = gc_SyringeSedativeItem;
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

function gc_SyringeeSedativeImage::onMount(%this,%obj,%slot) { bottomPrint(%obj.client,"<color:ffff00>[LMB]<color:00ff00> use on others <color:ffff00>[RMB]<color:00ff00> use on self",5); }
function gc_SyringeeSedativeImage::onCharge(%this,%obj,%slot) { %obj.playThread(2,spearReady); }
function gc_SyringeeSedativeImage::onAbortCharge(%this,%obj,%slot) { %obj.playthread(2,root); }

function gc_SyringeSedativeImage::onFire(%this,%obj,%slot)
{
  %obj.playThread(2,spearThrow);
  %raycast = containerRayCast(%obj.getEyePoint(),vectorAdd(%obj.getEyePoint(),vectorScale(%obj.getEyeVector(),3)),$TypeMasks::PlayerObjectType,%obj);
  %col = firstWord(%raycast);
  if(isObject(%col) && %col.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
  {
      %obj.spawnExplosion(gc_SyringeSedativeEffect,"1 1 1");
      if(%col.getDataBlock().getName() $= "gc_SedativePlayer")
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
      if(%col.getDataBlock().getName() $= "gc_AdrenalinePlayer")
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
      %col.setDataBlock("gc_SedativePlayer");
      %col.ResetDataBlockLater = %col.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%col.lastDatablock);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}

function gc_SyringeSedativeImage::onSelfUse(%this,%obj,%slot)
{
  if(%obj.getDataBlock().shapeFile $= "base/data/shapes/player/m.dts")
  {
      %obj.playThread(2,spearThrow);
      %obj.spawnExplosion(gc_SyringeSedativeEffect,"1 1 1");
      if(%obj.getDataBlock().getName() $= "gc_SedativePlayer")
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
      if(%obj.getDataBlock().getName() $= "gc_AdrenalinePlayer")
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
      %obj.setDataBlock("gc_SedativePlayer");
      %obj.ResetDataBlockLater = %obj.schedule(1000 * $GCStuff::SyringeDuration,setDatablock,%obj.lastDatablock);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}
