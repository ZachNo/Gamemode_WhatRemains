//What Remains weather system!!1!
//Much zomplex
//no speleple
//by ZSNO

//Change $Survive::globalWetInc on every function
//%timeMS time to complete move to %intensity
//%intensity self explainitory
//%cloudsOnly no rain, only cloudy

//TODO:
// - fix random parts of not actually happening i.e. clouds popping in, lightning not starting, etc.
// - change $Survive::globalWetInc

//BROKEN DUE TO UPDATE TO CLOUDS!

$Weather::debug = 1;

$Weather::lightning = 0;
$Weather::isCloudy = 0;
$Weather::isRaining = 0;
$Weather::interations = 0;
$Weather::rainHeight = 10;
$Weather::rainMinHeight = 0.5;
$Weather::newSunColor = "0.2 0.2 0.2";
$Weather::newSunAmbient = "0.1 0.1 0.1";
$Weather::newSunShadow = "0.09 0.09 0.09";
$Weather::updateMS = 1000;

$Weather::rainChance = 5;
$Weather::cloudyChance = 15;
$Weather::stopRainingChance = 75;
$Weather::keepCloudyChance = 80;
$Weather::modifyChance = 90;
$Weather::delayChangeRain = 60000;
$Weather::delayChangeNormal = 300000;
$Weather::randomStormInterval = "10 60";

//Vars used for sun tweening
$Weather::sunColT = "0 0 0";
$Weather::sunAmbT = "0 0 0";
$Weather::sunShaT = "0 0 0";

//Vars used to store original sun
$Weather::oldSunColor = "0 0 0";
$Weather::oldSunAmbient = "0 0 0";
$Weather::oldSunShadow = "0 0 0";


//Lightning stolen from some old lightning mod
datablock AudioDescription(AudioLightning3d : AudioDefault3d)
{
  maxDistance = 5000;
  referenceDistance = 15;
  is3d = false;
  volume = 1;
};
datablock AudioProfile(ThunderCrash1Sound)
{
  filename = "./thunder1.wav";
  description = AudioLightning3d;
};
datablock AudioProfile(ThunderCrash2Sound : ThunderCrash1Sound) { filename = "./thunder2.wav"; };
datablock AudioProfile(ThunderCrash3Sound : ThunderCrash1Sound) { filename = "./thunder3.wav"; };
datablock AudioProfile(ThunderCrash4Sound : ThunderCrash1Sound) { filename = "./thunder4.wav"; };
datablock AudioProfile(ThunderStrikeSound) { filename = "./thunderstrike.wav"; description = AudioClose3d; };

//### Effects

datablock ParticleData(LightningExplosionParticle)
{
  dragCoefficient = 0;
  gravityCoefficient = 1;
  inheritedVelFactor = 0;
  constantAcceleration = 0;
  lifetimeMS = 1000;
  lifetimeVarianceMS = 500;
  textureName = "./sparks";
  spinSpeed = 0;
  spinRandomMin = -10;
  spinRandomMax = 10;
  colors[0] = "1 0.6 0.2 1";
  colors[1] = "0 0 1 0";
  sizes[0] = 4;
  sizes[1] = 2;
  useInvAlpha = false;
};

datablock ParticleEmitterData(LightningExplosionEmitter)
{
  uiName = "";
  ejectionPeriodMS = 2;
  periodVarianceMS = 0;
  ejectionVelocity = 10;
  velocityVariance = 5;
  ejectionOffset = 2.5;
  thetaMin = 0;
  thetaMax = 180;
  phiReferenceVel = 0;
  phiVariance = 360;
  overrideAdvance = false;
  orientParticles = true;
  particles = "LightningExplosionParticle";
};

datablock ParticleData(LightningSmokeParticle)
{
  dragCoefficient = 4;
  gravityCoefficient = 0.5;
  inheritedVelFactor = 0.2;
  constantAcceleration = 0;
  lifetimeMS = 3500;
  lifetimeVarianceMS = 1500;
  textureName = "base/data/particles/cloud";
  spinSpeed = 0;
  spinRandomMin = -1;
  spinRandomMax = 1;
  colors[0] = "0 0.5 1 1";
  colors[1] = "0 0 0 0.5";
  colors[2] = "0 0 0 0";
  sizes[0] = 10;
  sizes[1] = 11;
  sizes[2] = 14;
  times[0] = 0;
  times[1] = 0.15;
  times[2] = 1;
  useInvAlpha = true;
  windCoefficient = 1;
};

datablock ParticleEmitterData(LightningSmokeEmitter)
{
  uiName = "";
  ejectionPeriodMS = 8;
  periodVarianceMS = 0;
  ejectionVelocity = 10;
  velocityVariance = 5;
  ejectionOffset = 2.5;
  thetaMin = 0;
  thetaMax = 180;
  phiReferenceVel = 0;
  phiVariance = 360;
  overrideAdvance = false;
  particles = "LightningSmokeParticle";
};

datablock ExplosionData(LightningExplosion)
{
  lifeTimeMS = 150;
  emitter[0] = LightningExplosionEmitter;
  emitter[1] = LightningSmokeEmitter;
  faceViewer = true;
  explosionScale = "1 1 1";
  lightStartRadius = 10;
  lightEndRadius = 0;
  lightStartColor = "1 0.6 0.2";
  lightEndColor = "0 0 0";
  damageRadius = 8;
  radiusDamage = 100;
  impulseRadius = 8;
  impulseForce = 100;
  soundProfile = ThunderStrikeSound;
  shakeCamera = true;
  camShakeFreq = "5 5 5";
  camShakeAmp = "8 8 8";
  camShakeDuration = 2;
  camShakeRadius = 16;
};

//### Projectiles/etc.

AddDamageType("Lightning",'<bitmap:Add-Ons/Gamemode_WR/CI_lightning> %1','%2 <bitmap:Add-Ons/Gamemode_WR/CI_lightning> %1',0.2,1);

datablock ProjectileData(LightningProjectile)
{
  uiName = "";
  directDamageType = $DamageType::Lightning;
  radiusDamageType = $DamageType::Lightning;
  explosion = LightningExplosion;
  lifetime = 1;
  fadeDelay = 1;
  explodeOnDeath = true;
};

datablock LightningData(LightningStorm)
{
  strikeTextures[0] = "Add-Ons/Server_ThunderStorm/lightning1frame1.png";
  strikeTextures[1] = "Add-Ons/Server_ThunderStorm/lightning1frame2.png";
  strikeTextures[2] = "Add-Ons/Server_ThunderStorm/lightning1frame3.png";
  thunderSounds[0] = ThunderCrash1Sound;
  thunderSounds[1] = ThunderCrash2Sound;
  thunderSounds[2] = ThunderCrash3Sound;
  thunderSounds[3] = ThunderCrash4Sound;
  directDamageType = $DamageType::Lightning;
  directDamage = 500;
};

function LightningData::applyDamage(%data,%lightning,%target,%position,%normal)
{
  %p = new Projectile()
  {
    dataBlock = LightningProjectile;
    initialVelocity = 0;
    initialPosition = %position;
  };
  MissionCleanup.add(%p);
//  %target.spawnExplosion(rocketLauncherProjectile,"1 1 1");
  %target.damage(%lightning,%position,%data.directDamage,%data.directDamageType);
}

function startLightning()
{
	$Weather::Lightning = 1;
	$Thunder = schedule(getRandom(1000,50000),0,Thunderstorm);
	if(isObject($Lightning))
		return;
	$Lightning = new Lightning() {
		position = "2048 2048 2000";
        rotation = "1 0 0 0";
        scale = "4096 4096 2000";
        dataBlock = "LightningStorm";
        strikesPerMinute = "0.0000000001";
        strikeWidth = "10";
        strikeRadius = "256";
        color = "1 1 1 1";
        fadeColor = "0.5 0.75 1 1";
        chanceToHitTarget = "0.5";
        boltStartRadius = "20";
        useFog = "0";
	};
	missionGroup.add($Lightning);
	$Lightning.strikesPerMinute = 0.0000000001;
}

function Thunderstorm()
{
  cancel($Thunder);
  $Thunder = schedule(getRandom(1000,50000),0,Thunderstorm);
  schedule(500,0,LightningResetFlash,missionGroup.getObject(1).color);
  sun.color = "1 1 1 1";
  sun.sendupdate();
  serverPlay3D("ThunderCrash" @ getRandom(1,4) @ "Sound","0 0 0");
  $Lightning.strikeRandomPoint();
}

function stopLightning()
{
	cancel($Thunder);
	$Lightning.delete();
	$Weather::Lightning = 0;
}

function LightningResetFlash(%color)
{
	sun.color = %color;
	sun.sendupdate();
}

//Blockland is dumb and sometimes STILL doesn't update the position of the sound
function rainSoundHack()
{
	rain_Sound.setTransform("0 0" SPC $Weather::rainHeight + $Weather::rainMinHeight);
}

function startStorm(%timeMS, %intensity, %cloudsOnly)
{
	//Create rain stuffs when not only clouds
	if(!%cloudsOnly)
	{
		//Make rain sound, put it up high to reduce volume
		new AudioEmitter(Rain_Sound)
		{
			position = "0 0 0";
			profile = musicData_Rain;
			useProfileDescription = 0;
			description = AudioLooping2D;
			type = "0";
			volume = 1;
			outsideAmbient = "1";
			ReferenceDistance = "4";
			maxDistance = "9001";
			isLooping = 1;
			is3D = 0;
		};
		Rain_Sound.setScopeAlways();
		//Create Rain precipitation
		new Precipitation(Rain) {
		   position = "0 0 0";
		   rotation = "1 0 0 0";
		   scale = "1 1 1";
		   dataBlock = "dummyPlayer";
		   dropTexture = "Add-Ons/Sky_DynamicWR/rain.png";
		   splashTexture = "";
		   dropSize = "0.75";
		   splashSize = "0";
		   useTrueBillboards = "1";
		   splashMS = "0";
		   minSpeed = "1.5";
		   maxSpeed = "2";
		   minMass = "0.75";
		   maxMass = "0.85";
		   maxTurbulence = "0.1";
		   turbulenceSpeed = "0.1";
		   rotateWithCamVel = "1";
		   useTurbulence = "1";
		   numDrops = "10000";
		   boxWidth = "50";
		   boxHeight = "50";
		   doCollision = "1";
		};
		MissionGroup.add(Rain);
		MissionGroup.add(Rain_Sound);
		
		//Rain sound is automatically really loud and ignores position, transform it in another function REALLY FAST to fix it
		schedule(10,0,rainSoundHack);
		
		//Set rain to 0, then tell it to change to intensity over timems
		Rain.setPercentange(0);
		Rain.schedule(100,modifyStorm,%intensity,%timeMS);
		
		//What Remains wetness moodle stuff
		$Survive::globalWetInc = %intensity;
		globalRainTick();
		
		//Sound volume tweening
		%tweenFactor = -(%intensity / (%timeMS / $Weather::updateMS));
		%sound = 1;
		
		%fogTween = %tweenFactor * $Weather::updateMS;
		
		//Keep track that it IS raining
		$Weather::isRaining = 1;
		if(%intensity >= 0.89)
			schedule(%timeMS,0,startLightning);
	}
	else
	{
		//If it's only clouds we don't need sound
		%sound = 0;
		%tweenFactor = 0;
		%fogTween = 0;
	}
	
	//Number of times to loop the tween
	$Weather::iterations = %timeMS/$Weather::updateMS;
	
	if(!$Weather::isCloudy)
	{
		//If it's not cloudy yet, do the clouds too + change lighting to darker
		//Sky.schedule(100,stormClouds,1, (%timeMS/$Weather::updateMS)); stormClouds broken
		//lighting tween
		%light = 1;

		//Vars used to store original sun
		$Weather::oldSunColor = sun.color;
		$Weather::oldSunAmbient = sun.ambient;
		$Weather::oldSunShadow = sun.shadowColor;
		
		//Vars used for sun tweening
		%colTemp = vectorSub($Weather::newSunColor,sun.color);
		%colTemp = getWord(%colTemp,0)/$Weather::iterations SPC getWord(%colTemp,1)/$Weather::iterations SPC getWord(%colTemp,2)/$Weather::iterations;
		%ambTemp = vectorSub($Weather::newSunAmbient,sun.ambient);
		%ambTemp = getWord(%ambTemp,0)/$Weather::iterations SPC getWord(%ambTemp,1)/$Weather::iterations SPC getWord(%ambTemp,2)/$Weather::iterations;
		%shaTemp = vectorSub($Weather::newSunShadow,sun.shadowColor);
		%shaTemp = getWord(%shaTemp,0)/$Weather::iterations SPC getWord(%shaTemp,1)/$Weather::iterations SPC getWord(%shaTemp,2)/$Weather::iterations;
		
		$Weather::sunColT = %colTemp;
		$Weather::sunAmbT = %ambTemp;
		$Weather::sunShaT = %shaTemp;
	}
	else
		%light = 0;
	
	//Start weather tweening
	schedule($Weather::updateMS, 0, stormTweening, %tweenFactor, %sound, %light, %fogTween);
	
	//Keep track that it is cloudy
	$Weather::isCloudy = 1;
}

//Change the storm while it's storming
function changeStorm(%timeMS, %intensity)
{
	//Change amount of rain
	Rain.modifyStorm(%intensity,%timeMS);
	$Survive::globalWetInc = %intensity * 2;
	
	//Tween sound
	$Weather::iterations = %timeMS/$Weather::updateMS;
	%sub = %intensity - (1 - (getWord(Rain_Sound.getTransform(),2)-$Weather::rainMinHeight)/$Weather::rainHeight);
	%tweenFactor = -(%sub / (%timeMS/$Weather::updateMS));
	%fogTween = %tweenFactor * $Weather::updateMS;
	stormTweening(%tweenFactor, 1, 0, %fogTween);
	if(!$Weather::lightning && %intensity >= 0.9)
		schedule(%timeMS,0,startLightning);
	if($Weather::lightning && %intensity < 0.9)
	{
		$Weather::lighting = 0;
		cancel($Thunder);
		$Lightning.delete();
	}
}

//Stop the storm
function stopStorm(%timeMS, %rainOnly)
{
	if(isObject(Rain))
	{
		Rain.modifyStorm(0,%timeMS);
		Rain.schedule(%timeMS + 100,delete);

		%tweenFactor = (1 - (getWord(Rain_Sound.getTransform(),2) - $Weather::rainMinHeight)/$Weather::rainHeight) / (%timeMS/$Weather::updateMS);
		
		Rain_Sound.schedule(%timeMS + 100,delete);
		
		%fogTween = %tweenFactor * $Weather::updateMS;
	}
	
	$Weather::iterations = %timeMS/$Weather::updateMS;
	
	if(!%rainOnly)
	{
		//Sky.stormClouds(0,%timeMS/$Weather::updateMS);
		%light = 1;
		
		//Vars used for sun tweening
		%colTemp = vectorSub($Weather::oldSunColor,sun.color);
		%colTemp = getWord(%colTemp,0)/$Weather::iterations SPC getWord(%colTemp,1)/$Weather::iterations SPC getWord(%colTemp,2)/$Weather::iterations;
		%ambTemp = vectorSub($Weather::oldSunAmbient,sun.ambient);
		%ambTemp = getWord(%ambTemp,0)/$Weather::iterations SPC getWord(%ambTemp,1)/$Weather::iterations SPC getWord(%ambTemp,2)/$Weather::iterations;
		%shaTemp = vectorSub($Weather::oldSunShadow,sun.shadowColor);
		%shaTemp = getWord(%shaTemp,0)/$Weather::iterations SPC getWord(%shaTemp,1)/$Weather::iterations SPC getWord(%shaTemp,2)/$Weather::iterations;
		
		$Weather::sunColT = %colTemp;
		$Weather::sunAmbT = %ambTemp;
		$Weather::sunShaT = %shaTemp;
		
		$Weather::isCloudy = 0;
	}
	else
		%light = 0;
	
	if($Weather::lightning)
	{
		$Weather::lightning = 0;
		cancel($Thunder);
		$Lightning.delete();
	}
	
	stormTweening(%tweenFactor, isObject(Rain_Sound), %light, %fogTween);
	$Weather::isRaining = 0;
	cancel($wetTick);
	
	%count = ClientGroup.getCount();
	for(%i = 0;%i < %count;%i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(isObject(%cl.player))
		{
			%cl.player.deWetSch = %cl.player.schedule(5000,decreaseWet);
		}
	}
}

//Used to manually tween lighting and sound volume
function stormTweening(%tweenFactor, %sound, %lighting, %fogTween)
{
	cancel($Weather::tween);
	
	if(%fogTween != 0)
	{
		sky.visibleDistance += %fogTween;
		sky.sendUpdate();
	}	
	if(%lighting)
	{
		sun.color = vectorAdd($Weather::sunColT,sun.color);
		sun.ambient = vectorAdd($Weather::sunAmbT,sun.ambient);
		sun.shadowColor = vectorAdd($Weather::sunShaT,sun.shadowColor);
		sun.sendUpdate();
	}
	if(%sound)
	{
		%vol = (getWord(Rain_Sound.getTransform(),2) - $Weather::rainMinHeight)/$Weather::rainHeight + %tweenFactor;
		if(%vol > 1)
			%vol = 1;
		if(%vol < 0)
			%vol = 0;
		Rain_Sound.setTransform("0 0" SPC %vol*$Weather::rainHeight + $Weather::rainMinHeight);
	}
	$Weather::iterations--;
	if($Weather::iterations > 0)
		$Weather::tween = schedule($Weather::updateMS,0,stormTweening,%tweenFactor,%sound,%lighting, %fogTween);
	else
	{
		if($Weather::isCloudy)
		{
			sun.color = $Weather::newSunColor;
			sun.ambient = $Weather::newSunAmbient;
			sun.shadowColor = $Weather::newSunShadow;
			sun.sendUpdate();
		}
		else
		{
			sun.color = $Weather::oldSunColor;
			sun.ambient = $Weather::oldSunAmbient;
			sun.shadowColor = $Weather::oldSunShadow;
			sun.sendUpdate();
		}
	}
	
	if($Weather::Debug)
		echo("stormTweening(" @ %tweenFactor @ "," @ %sound @ "," @ %lighting @ "," @ %fogTween @ ");" SPC $Weather::iterations SPC "iterations remaining...");
}

//Randomizes weather
function randomWeather()
{
	if($Weather::Debug)
		echo("Random Weather Loop");
		
	cancel($Weather::randomLoop);
	if(!$Weather::isRaining)
	{
		if($Weather::rainChance >= getRandom(1,100))
		{
			startStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, getRandom(10,100)/100, 0);
			$Weather::randomLoop = schedule($Weather::delayChangeRain, 0, randomWeather);
		}
		else if(!$Weather::isCloudy && $Weather::cloudyChance >= getRandom(1,100))
		{
			startStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, 0, 1);
			$Weather::randomLoop = schedule($Weather::delayChangeNormal, 0, randomWeather);
		}
		else if($Weather::isCloudy && $Weather::keepCloudyChance >= getRandom(1,100))
		{
			stopStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, 0);
			$Weather::randomLoop = schedule($Weather::delayChangeNormal, 0, randomWeather);
		}
		else
		{
			$Weather::randomLoop = schedule($Weather::delayChangeNormal, 0, randomWeather);
		}
	}
	else
	{
		if($Weather::stopRainingChance >= getRandom(1,100))
		{
			if($Weather::keepCloudyChance >= getRandom(1,100))
			{
				stopStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, 1);
			}
			else
			{
				stopStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, 0);
			}
			$Weather::randomLoop = schedule($Weather::delayChangeNormal, 0, randomWeather);
		}
		else if($Weather::modifyChance >= getRandom(1,100))
		{
			changeStorm(getRandom(getWord($Weather::randomStormInterval,0),getWord($Weather::randomStormInterval,1)) * 1000, getRandom(10,100)/100);
			$Weather::randomLoop = schedule($Weather::delayChangeRain, 0, randomWeather);
		}
		else
		{
			$Weather::randomLoop = schedule($Weather::delayChangeRain, 0, randomWeather);
		}
	}
}

//Init skybox and junk
function initWeather()
{
	loadDayCycle("Add-ons/DayCycle_Teneksi/siten1.daycycle");
	new scriptObject(fake){isAdmin=1;isSuperAdmin=1;};
	serverCmdEnvGui_SetVar(fake, "DayLength", 7200);
	//sky.stormclouds(0,10);
	sky.fogDistance = 0;
	
	randomWeather();
}

//Take out weather for now
//schedule(100,0,initWeather);