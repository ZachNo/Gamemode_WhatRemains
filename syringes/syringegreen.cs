//########## Syringe Green (Hallucinogen)

//### Item

datablock ParticleData(gc_SyringeHallucinogenParticle)
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
  colors[0] = "0 1 0 0";
  colors[1] = "0 1 0 1";
  colors[2] = "0 1 0 0";
  sizes[0] = 0.2;
  sizes[1] = 0.4;
  sizes[2] = 0.2;
  times[0] = 0;
  times[1] = 0.5;
  times[2] = 1;
  useInvAlpha = false;
};

datablock ParticleEmitterData(gc_SyringeHallucinogenEmitter)
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
  particles = "gc_SyringeHallucinogenParticle";
};

datablock ExplosionData(gc_SyringeHallucinogenExplosion)
{
  lifeTimeMS = 200;
  emitter[0] = gc_SyringeHallucinogenEmitter;
  faceViewer = true;
  explosionScale = "1 1 1";
};

datablock ProjectileData(gc_SyringeHallucinogenEffect)
{
  uiName = "";
  explosion = gc_SyringeHallucinogenExplosion;
  lifetime = 1;
  fadeDelay = 1;
  explodeOnDeath = true;
};

datablock ItemData(gc_SyringeHallucinogenItem)
{
  uiName = "Syringe Hallucinogen";
  iconName = "./icon_syringegreen";
  image = gc_SyringeHallucinogenImage;
  category = Weapon;
  className = Weapon;
  shapeFile = "./syringegreen.dts";
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

datablock shapeBaseImageData(gc_SyringeHallucinogenImage)
{
  shapeFile = "./syringegreen.dts";
  emap = true;
  correctMuzzleVector = false;
  className = "WeaponImage";
  item = gc_SyringeHallucinogenItem;
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

function gc_SyringeHallucinogenImage::onMount(%this,%obj,%slot) { bottomPrint(%obj.client,"<color:ffff00>[LMB]<color:00ff00> use on others <color:ffff00>[RMB]<color:00ff00> use on self",5); }
function gc_SyringeHallucinogenImage::onCharge(%this,%obj,%slot) { %obj.playThread(2,spearReady); }
function gc_SyringeHallucinogenImage::onAbortCharge(%this,%obj,%slot) { %obj.playthread(2,root); }

function gc_SyringeHallucinogenEffect(%obj,%duration)
{
  if(!isObject(%obj)) return;
  if(!isObject(%obj.client)) return;
  cancel(%obj.HallucinogenEffect);
  if(%duration < 0) { %obj.client.setControlCameraFov(90); return; }
  %obj.client.setControlCameraFov(140 + (getRandom()*20));
  %duration -= 1;
  %obj.HallucinogenEffect = schedule(100,0,gc_SyringeHallucinogenEffect,%obj,%duration);

}

function gc_SyringeHallucinogenImage::onFire(%this,%obj,%slot)
{
  %obj.playThread(2,spearThrow);
  %raycast = containerRayCast(%obj.getEyePoint(),vectorAdd(%obj.getEyePoint(),vectorScale(%obj.getEyeVector(),3)),$TypeMasks::PlayerObjectType,%obj);
  %col = firstWord(%raycast);
  if(isObject(%col))
  {
      cancel(%col.HallucinogenEffect);
      %obj.spawnExplosion(gc_SyringeHallucinogenEffect,"1 1 1");
      cancel(%col.HallucinogenEffect);
      %col.HallucinogenEffect = schedule(100,0,gc_SyringeHallucinogenEffect,%col,$GCStuff::SyringeDuration * 10);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}

function gc_SyringeHallucinogenImage::onSelfUse(%this,%obj,%slot)
{
      %obj.playThread(2,spearThrow);
      %obj.spawnExplosion(gc_SyringeHallucinogenEffect,"1 1 1");
      cancel(%obj.HallucinogenEffect);
      %obj.HallucinogenEffect = schedule(100,0,gc_SyringeHallucinogenEffect,%obj,$GCStuff::SyringeDuration * 10);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
}
