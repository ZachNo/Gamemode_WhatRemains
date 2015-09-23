//Originally by MARBLEMAN
//Modified by ZSNO

function Player::BlockEyes(%this)
{
   %this.lastblackData = %this.getDatablock();
   %this.setDatablock(PlayerFirstPersonArmor); 
   %this.schedule(10,constantBlockEyes);
}
function Player::UnBlockEyes(%this)
{
   cancel(%this.blockSch);
   %this.setDatablock(%this.lastblackdata);
   %this.lastBlackData = "";
}

function Player::constantBlockEyes(%this)
{
	cancel(%this.blockSch);
	%this.setWhiteout(1);
	%this.blockSch = %this.schedule(50,constantBlockEyes);
}

datablock PlayerData(PlayerFirstPersonArmor : PlayerStandardArmor)
{
   firstPersonOnly = 1;
   canjet = 0;
   uiname = "";
};

datablock WheeledVehicleData(SleepingVehicle)
{
	//tagged fields
	doSimpleDismount = false;		//just unmount the player, dont look for a free space
	maxDismountDist = 5;

   numMountPoints = 1;

   category = "Vehicles";
   shapeFile = "add-ons/Item_skis/deathvehicle.dts";
   emap = true;

   maxDamage = 1000.00;
   destroyedLevel = 10.00;

   uiName = "";
   rideable = false;

   maxSteeringAngle = 1.800;  // Maximum steering angle, should match animation

   // 3rd person camera settings
   cameraRoll = true;         // Roll the camera with the vehicle?
   cameraMaxDist = 7.5;         // Far distance from vehicle
   cameraOffset = 4.4;        // Vertical offset from camera mount point
   cameraLag = 25.0;           // Velocity lag of camera
   cameraDecay = 1.75;  //0.75;      // Decay per sec. rate of velocity lag
   cameraTilt = 0.3201; //tilt adjustment for camera: ~20 degrees down

   numWheels = 0;
   
   // Rigid Body
   mass = 300;
   density = 5.0;
   massCenter = "0 0 1";    // Center of mass for rigid body
   massBox = "1.2 1.2 1.2";         // Size of box used for moment of inertia,
                              // if zero it defaults to object bounding box
   drag = 0.2; //was 0.8                // Drag coefficient
   bodyFriction = 0.21; //was 0.21
   bodyRestitution = 0.2; //was 0.2
   minImpactSpeed = 0;        // Impacts over this invoke the script callback
   softImpactSpeed = 3;       // Play SoftImpact Sound
   hardImpactSpeed = 10;      // Play HardImpact Sound
   integration = 4;           // Physics integration: TickSec/Rate
   collisionTol = 0.25;        // Collision distance tolerance
   contactTol = 0.000000000000000000000001;          // Contact velocity tolerance
   justcollided = 0;

   isSled = true;	//if its a sled, the wing surfaces dont work unless its on the ground
	
   // Engine
   engineTorque = 0;       // Engine power
   engineBrake = 0;         // Braking when throttle is 0
   brakeTorque = 80000;        // When brakes are applied
   maxWheelSpeed = 30;        // Engine scale by current speed / max speed

	forwardThrust		= 00; //5000
	reverseThrust		= 00;
	lift				= 00;
	maxForwardVel		= 30;
	maxReverseVel		= 30;
	horizontalSurfaceForce	= 50; //50
	verticalSurfaceForce	= 50;
	rollForce		= 0; //was 9000
	yawForce		= 0; //was 9000
	pitchForce		= 0; //was 36000
	rotationalDrag	= 5; //1
	stallSpeed		= 0;

	minRunOverSpeed    = 4;   //how fast you need to be going to run someone over (do damage)
	runOverDamageScale = 0;   //when you run over someone, speed * runoverdamagescale = damage amt
	runOverPushScale   = 1.2; //how hard a person you're running over gets pushed
};
function Player::enterSleep(%this)
{
   //make a new ski vehicle and mount the player on it
   %this.sleeping = 1;
   %client = %this.client;
   %pos = %this.getEyeTransform();
   %this.canDismount = false;
   %vehicle = new WheeledVehicle() 
   {
      dataBlock = SleepingVehicle;
      client = %client;
   };
   MissionCleanup.add(%vehicle);
   %vehicle.setTransform(%pos);
   %vehicle.schedule(5,mountObject,%this,0);
   %this.sleepvehicle = %vehicle;
   %this.blockEyes();
   %this.sleepTick();
   return;
}
function Player::SleepTick(%this)
{
   cancel(%this.sleepingtick);
   if(!%this.sleeping)
      return;
   %this.addTired(1);
   if(%this.isPassedout == 1 && %this.tired >= 25)
   {
      %this.leaveSleep();
      return;
   }
	if(%this.isPassedout == 2)
	{
		if(%this.passedoutticks < 0)
		{
			%this.leaveSleep();
			return;
		}
		else
			%this.passedoutTicks--;
	}		
   if(isObject(%this.client))
      %this.client.centerprint("\c5Sleeping...<br>\c5Stamina: \c0" @ %this.tired,3);
   if(%this.tired == 100)
   {
	messageClient(%this.client,'',"Your stamina is full, waking up.");
	servercmdWakeup(%this.client);
   }
      
   %this.sleepingtick = %this.schedule(3000,sleepTick);
}
function Player::LeaveSleep(%this)
{
   if(!isObject(%vehicle=%this.sleepVehicle) || !%this.sleeping)
      return;
   cancel(%this.sleepingTick);
   %this.canDismount = true;
   %this.dismount();
   %this.sleeping = 0;
   %this.unblockeyes();
   %this.ispassedout = 0;
}

package sleeping
{
   function servercmdunUseTool(%c)
   {
      if(isObject(%p = %c.player))
         if(%p.sleeping)
            return;
      parent::ServercmdunUseTool(%c);
   }
   function SleepingVehicle::onDriverLeave(%this,%obj,%player)
   {
      parent::onDriverLeave(%this,%obj,%player);
      %obj.schedule(10,delete);
   }
   function servercmdUseTool(%c,%i)
   {
      if(isObject(%p = %c.player))
         if(%p.sleeping)
            return;
      parent::ServercmdUseTool(%c,%i);
   }
};
activatepackage(sleeping);

function servercmdSleep(%c)
{
   if(!isObject(%p = %c.player))
   {
      %c.chatMessage("\c5You can't sleep right now...");
      return;
   }
   if(%p.sleeping)
   {
      %c.chatMessage("\c5You are already sleeping");
      return;
   }
	%pos = %p.getPosition();
	%end = getWords(%pos,0,1) SPC getWord(%pos,2) + 10;
	%ray = ContainerRayCast(%pos, %end, $TypeMasks::FxBrickAlwaysObjectType, %this);
   if(!isObject(%ray))
	{
		messageClient(%c,'',"\c5You need to sleep under something!");
		return;
	}
	if(%p.tired >= 100)
	{
		messageClient(%c,'',"\c5Already have full sleep!");
		return;
	}
   %c.chatMessage("\c5Use \"\c3/wakeup\c5\" to wake up.");
   %p.sleepStart = getSimTime();
   %p.enterSleep();
}
function servercmdWakeUp(%c)
{
	%p = %c.player;
	if(%p.isPassedOut)
		return;
   if(!isObject(%p) || !%p.sleeping)
   {
      %c.chatMessage("\c5You aren't sleeping");
      return;
   }
   if(%p.sleepStart+3000>getSimTime())
   {
      %c.chatMessage("\c5You haven't even slept for 3 seconds!");
      return;
   }
   if(%p.tired != 100)
   {
      %c.chatMessage("\c5You need to sleep until you aren't tired anymore! (100)");
      return;
   }
   %p.leaveSleep();
}

datablock ParticleData(SleepParticle)
{
   dragCoefficient      = 5.0;
   gravityCoefficient   = -0.2;
   inheritedVelFactor   = 0.0;
   constantAcceleration = 0.0;
   lifetimeMS           = 1000;
   lifetimeVarianceMS   = 500;
   useInvAlpha          = false;
   textureName          = "./Zzz";
   colors[0]     = "1 1 1 1";
   colors[1]     = "1 1 1 1";
   colors[2]     = "1 1 1 1";
   sizes[0]      = 0.4;
   sizes[1]      = 0.6;
   sizes[2]      = 0.4;
   times[0]      = 0.0;
   times[1]      = 0.2;
   times[2]      = 1.0;
};

datablock ParticleEmitterData(SleepEmitter)
{
   ejectionPeriodMS = 20;
   periodVarianceMS = 0;
   ejectionVelocity = 0.5;
   ejectionOffset   = 1.0;
   velocityVariance = 0.49;
   thetaMin         = 0;
   thetaMax         = 120;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "SleepParticle";

   uiName = "Emote - Sleep";
};
