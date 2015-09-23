//########## Syringe Red (Poison)

//### Effects

datablock ParticleData(gc_SyringePoisonParticle)
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
  colors[0] = "0.5 0 1 0";
  colors[1] = "0.5 0 1 1";
  colors[2] = "0.5 0 1 0";
  sizes[0] = 0.2;
  sizes[1] = 0.4;
  sizes[2] = 0.2;
  times[0] = 0;
  times[1] = 0.5;
  times[2] = 1;
  useInvAlpha = false;
};

datablock ParticleEmitterData(gc_SyringePoisonEmitter)
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
  particles = "gc_SyringePoisonParticle";
};

datablock ExplosionData(gc_SyringePoisonExplosion)
{
  lifeTimeMS = 200;
  emitter[0] = gc_SyringePoisonEmitter;
  faceViewer = true;
  explosionScale = "1 1 1";
};

datablock ProjectileData(gc_SyringePoisonEffect)
{
  uiName = "";
  explosion = gc_SyringePoisonExplosion;
  lifetime = 1;
  fadeDelay = 1;
  explodeOnDeath = true;
};

//### Item

datablock ItemData(gc_SyringePoisonItem)
{
  uiName = "Syringe Poison";
  iconName = "./icon_syringeviolet";
  image = gc_SyringePoisonImage;
  category = Weapon;
  className = Weapon;
  shapeFile = "./syringeviolet.dts";
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

datablock shapeBaseImageData(gc_SyringePoisonImage)
{
  shapeFile = "./syringeviolet.dts";
  emap = true;
  correctMuzzleVector = false;
  className = "WeaponImage";
  item = gc_SyringePoisonItem;
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

function gc_SyringePoisonImage::onMount(%this,%obj,%slot) { bottomPrint(%obj.client,"<color:ffff00>[LMB]<color:00ff00> use on others <color:ffff00>[RMB]<color:00ff00> use on self",5); }
function gc_SyringePoisonImage::onCharge(%this,%obj,%slot) { %obj.playThread(2,spearReady); }
function gc_SyringePoisonImage::onAbortCharge(%this,%obj,%slot) { %obj.playthread(2,root); }

function gc_SyringePoisonImage::onFire(%this,%obj,%slot)
{
  %obj.playThread(2,spearThrow);
  %raycast = containerRayCast(%obj.getEyePoint(),vectorAdd(%obj.getEyePoint(),vectorScale(%obj.getEyeVector(),3)),$TypeMasks::PlayerObjectType,%obj);
  %col = firstWord(%raycast);
  if(isObject(%col))
  {
      %col.spawnExplosion(gc_SyringePoisonEffect,"1 1 1");
      cancel(%col.gc_poisoning2);
      gc_poisoning2(%col);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
  }
}

function gc_SyringePoisonImage::onSelfUse(%this,%obj,%slot)
{
      %obj.playThread(2,spearThrow);
      %obj.spawnExplosion(gc_SyringePoisonEffect,"1 1 1");
      cancel(%obj.gc_poisoning2);
      gc_poisoning2(%obj);
      %currSlot = %obj.currTool;
      %obj.tool[%currSlot] = 0;
      %obj.weaponCount--;
      messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
      serverCmdUnUseTool(%obj.client);
}

function gc_poisoning2(%obj,%duration)
{
  if(!isObject(%obj)) return 0;
  cancel(%obj.gc_poisoning2);
  %obj.damage(%obj,%obj.getPosition(),2,$DamageType::gc_Syringe);
  %obj.gc_poisoning2 = schedule(1500,0,gc_poisoning2,%obj,%duration);
  %obj.setDamageFlash(%obj.getDamageFlash()+0.1);
  %obj.spawnExplosion(gc_SyringePoisonEffect,"1 1 1");
}
