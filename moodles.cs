//Survive "Moodles" mod
//Made by ZSNO
//could probably be optimized, but I don't feel like breaking it

//Stats:
//- Anxiety - (Done) - tested
//- Hunger 	- (done)
//- In pain - (done)
//- Sick 	- (done)
//- Infection (done)
//- Thirsty - (done)
//- Tired	- (done)
//- Wet		- (done)

//Cake - 10
//Candy - 5
//Cheeseburger - 30
//DoubleBurger - 40
//Fries - 15
//Hamburger - 25
//Hotdog - 20
//Icecreamcup - 5
//Icecreamstick - 5
//kabob - 15
//Sandwich - 20
//sushi roll - 10

//List of functions:
//globalHungerTick()
//globalThirstTick()
//globalRainTick()
//globalTiredTick()
//player::addHunger(%val)
//player::addThirst(%val)
//player::statusTick()
//player::addTired(%val)
//player::jitter(%intensity)
//player::rollForSickness()
//player::addSickness(%val)
//player::startSickTick()
//player::startInfection()
//player::addWet(%val)
//player::decreaseWet()
//player::deAnx()
//player::tryDeAnx()
//player::startAnxiety(%type,%zombie,%val)
//player::setInPain(%bool)
//player::displayStats()

datablock AudioProfile(drinkSound)
{
   filename    = "./foodMod/drink.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(eatSound)
{
	filename    = "./foodMod/eat.wav";
	description = AudioClose3d;
	preload = true;
};
exec("./foodMod/food.cs");
//ForceRequiredAddOn("Player_overTheShoulder");

datablock ParticleData(MoodleBleedData)
{
   textureName = "./m_bl.png";
};
datablock ParticleData(MoodleDamagedData)
{
   textureName = "./m_bd.png";
};
datablock ParticleData(MoodleHungerData)
{
   textureName = "./m_h.png";
};
datablock ParticleData(MoodleInfectedData)
{
   textureName = "./m_i.png";
};
datablock ParticleData(MoodleInPainData)
{
   textureName = "./m_ip.png";
};
datablock ParticleData(MoodlePassedOutData)
{
   textureName = "./m_po.png";
};
datablock ParticleData(MoodleSickData)
{
   textureName = "./m_s.png";
};
datablock ParticleData(MoodleStressedData)
{
   textureName = "./m_so.png";
};
datablock ParticleData(MoodleTerrifiedData)
{
   textureName = "./m_tr.png";
};
datablock ParticleData(MoodleThirstyData)
{
   textureName = "./m_ty.png";
};
datablock ParticleData(MoodleTiredData)
{
   textureName = "./m_ti.png";
};
datablock ParticleData(MoodleWetData)
{
   textureName = "./m_w.png";
};

//Player mods for slowerness
datablock PlayerData(SlowerPlayerOTSNoJet : PlayerOTSNoJet)
{
	uiname = " ";
	maxForwardSpeed = 5;
	maxForwardCrouchSpeed = 2;
	maxBackwardSpeed = 3;
	maxBackwardCrouchSpeed = 1;
	maxSideSpeed = 4;
	maxSideCrouchSpeed = 1;
};
datablock PlayerData(SlowestPlayerOTSNoJet : PlayerOTSNoJet)
{
	uiname = " ";
	maxForwardSpeed = 3;
	maxForwardCrouchSpeed = 1;
	maxBackwardSpeed = 2;
	maxBackwardCrouchSpeed = 1;
	maxSideSpeed = 2;
	maxSideCrouchSpeed = 1;
};

//Global vars for timeouts, ticks, etc.
$Survive::hungerTick 	= 300000;
$Survive::thirstTick 	= 180000;
$Survive::wetTick 		= 10000;
$Survive::anxietyTick 	= 3000;
$Survive::smallPainTimeout = 20000;
$Survive::statusTick 	= 3000;
$Survive::tiredTick		= 180000;
$Survive::painTime		= 10000;
$Survive::sicknessChance= 25;
$Survive::sicknessReroll= 5000;
$Survive::sickTick		= 15000;
$Survive::infectionChance = 20;
$Survive::globalWetInc	= 1;

//Global stats functions
function globalHungerTick()
{
	cancel($hungerTick);
	%count = ClientGroup.getCount();
	for(%i = 0;%i < %count;%i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(isObject(%cl.player))
		{
			%cl.player.addhunger(-1);
		}
	}
	$hungerTick = schedule($Survive::hungerTick,0,globalHungerTick);
}

function globalThirstTick()
{
	cancel($thirstTick);
	%count = ClientGroup.getCount();
	for(%i = 0;%i < %count;%i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(isObject(%cl.player))
		{
			%cl.player.addthirst(-1);
		}
	}
	$thirstTick = schedule($Survive::thirstTick,0,globalThirstTick);
}

function globalRainTick()
{
	cancel($wetTick);
	%count = ClientGroup.getCount();
	for(%i = 0;%i < %count;%i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(isObject(%cl.player))
		{
			%p = %cl.player;
			cancel(%p.deWetSch);
			%pos = %p.getPosition();
			%end = getWords(%pos,0,1) SPC getWord(%pos,2) + 10;
			%ray = ContainerRayCast(%pos, %end, $TypeMasks::FxBrickAlwaysObjectType, %this);
			if(!isObject(%ray))
				%cl.player.addWet($Survive::globalWetInc);
		}
	}
	$wetTick = schedule($Survive::wetTick,0,globalRainTick);
}

function globalTiredTick()
{
	cancel($tiredTick);
	%count = ClientGroup.getCount();
	for(%i = 0;%i < %count;%i++)
	{
		%cl = ClientGroup.getObject(%i);
		if(isObject(%cl.player))
			%cl.player.addTired(-1);
	}
	$tiredTick = schedule($Survive::tiredTick,0,globalTiredTick);
}

RegisterPersistenceVar("anxiety",false,"");
RegisterPersistenceVar("hunger",false,"");
RegisterPersistenceVar("sick",false,"");
RegisterPersistenceVar("thirst",false,"");
RegisterPersistenceVar("tired",false,"");
RegisterPersistenceVar("wet",false,"");
RegisterPersistenceVar("health",false,"");
RegisterPersistenceVar("infection",false,"");
RegisterPersistenceVar("isSick",false,"");
RegisterPersistenceVar("isInfected",false,"");
RegisterPersistenceVar("isPassedout",false,"");


//Jitter things
datablock ExplosionData(jitter1Explosion)
{
   lifeTimeMS = 100;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "1.0 1.0 1.0";
   camShakeAmp = "0.5 0.5 0.5";
   camShakeDuration = 0.1;
   camShakeRadius = 0.1;
};

datablock ExplosionData(jitter2Explosion : jitter1Explosion)
{
   camShakeAmp = "2 2 2";
   camShakeDuration = 0.2;
};

datablock ExplosionData(jitter3Explosion : jitter1Explosion)
{
   camShakeAmp = "5 5 5";
   camShakeDuration = 0.5;
};

datablock ProjectileData(jitter1Projectile)
{
   projectileShapeName = "add-ons/weapon_gun/bullet.dts";
   explosion           = jitter1Explosion;
   collidewithplayers = 1;
   muzzleVelocity      = 1;
   velInheritFactor    = 1.0;

   armingDelay         = 00;
   lifetime            = 100;
   fadeDelay           = 20;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;
};

datablock ProjectileData(jitter2Projectile : jitter1Projectile)
{
   explosion           = jitter2Explosion;
};

datablock ProjectileData(jitter3Projectile : jitter1Projectile)
{
   explosion           = jitter3Explosion;
};

//Hunger
function player::addHunger(%this, %val)
{
	%this.hunger += %val;
	if(%this.hunger > 200)
		%this.hunger = 200;
	if(%this.hunger < 0)
		%this.hunger = 0;
	if(%this.hunger == 0)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = 3;
		%this.healthness = -1;
		if(%this.foodLevel != 1)
		{
	//		messageClient(%this.client,'',"\c6FEED ME MAN!");
			%this.foodLevel = 1;
		}
		%this.stat1 = "<bitmap:add-ons/gamemode_wr/m_h.png>";
	}
	else if(%this.hunger <= 25)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = 2;
		%this.healthness = 0;
		if(%this.foodLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6I am really hungry!");
			%this.foodLevel = 2;
		}
		%this.stat1 = "<bitmap:add-ons/gamemode_wr/m_h.png>";
	}
	else if(%this.hunger <= 50)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = 1;
		%this.healthness = 0;
		if(%this.foodLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6Getting pretty hungry now.");
			%this.foodLevel = 3;
		}
		%this.stat1 = "";
	}
	else if(%this.hunger <= 75)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = 1;
		%this.healthness = 0;
		if(%this.foodLevel != 4)
		{
	//		messageClient(%this.client,'',"\c6Could use a bite to eat.");
			%this.foodLevel = 4;
		}
		%this.stat1 = "";
	}
	else if(%this.hunger <= 100)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = -1;
		%this.healthness = 0;
		if(%this.foodLevel != 5)
		{
	//		messageClient(%this.client,'',"\c6I feel slightly fed.");
			%this.foodLevel = 5;
		}
		%this.stat1 = "";
	}
	else if(%this.hunger <= 125)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = -1;
		%this.healthness = 1;
		if(%this.foodLevel != 6)
		{
	//		messageClient(%this.client,'',"\c6I feel fed.");
			%this.foodLevel = 6;
		}
		%this.stat1 = "";
	}
	else if(%this.hunger <= 150)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = -2;
		%this.healthness = 2;
		if(%this.foodLevel != 7)
		{
	//		messageClient(%this.client,'',"\c6I feel well fed.");
			%this.foodLevel = 7;
		}
		%this.stat1 = "";
	}
	else if(%this.hunger <= 175)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sickness = -3;
		%this.healthness = 3;
		if(%this.foodLevel != 8)
		{
	//		messageClient(%this.client,'',"\c6I feel very well fed.");
			%this.foodLevel = 8;
		}
		%this.stat1 = "";
	}
}

//Thirst
function player::addThirst(%this, %val)
{
	%this.thirst += %val;
	if(%this.thirst > 100)
		%this.thirst = 100;
	if(%this.thirst < 0)
		%this.thirst = 0;
	if(%this.thirst == 0)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sicknessT = 3;
		%this.healthnessT = -1;
		if(%this.thirstLevel != 1)
		{
	//		messageClient(%this.client,'',"\c6I'm dying from dehydration! :C");
			%this.thirstLevel = 1;
			%this.setDatablock(SlowestPlayerOTSNoJet);
		}
		%this.stat2 = "<bitmap:add-ons/gamemode_wr/m_ty.png>";
	}
	else if(%this.thirst <= 25)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sicknessT = 2;
		%this.healthnessT = 0;
		if(%this.thirstLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6I'm parched!");
			%this.thirstLevel = 2;
			%this.setDatablock(SlowerPlayerOTSNoJet);
		}
		%this.stat2 = "<bitmap:add-ons/gamemode_wr/m_ty.png>";
	}
	else if(%this.thirst <= 50)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sicknessT = 1;
		%this.healthnessT = 0;
		if(%this.thirstLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6I feel thirsty.");
			%this.thirstLevel = 3;
			%this.setDatablock(PlayerOTSNoJet);
		}
		%this.stat2 = "";
	}
	else if(%this.thirst <= 75)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sicknessT = 0;
		%this.healthnessT = 0;
		if(%this.thirstLevel != 4)
		{
	//		messageClient(%this.client,'',"\c6I feel slightly thirsty.");
			%this.thirstLevel = 4;
		}
		%this.stat2 = "";
	}
	else if(%this.thirst >= 75)
	{
		if(!%this.statusTicked)
			%this.statusTick();
		%this.sicknessT = 0;
		%this.healthnessT = 0;
		if(%this.thirstLevel != 5)
		{
	//		messageClient(%this.client,'',"\c6I feel hydrated.");
			%this.thirstLevel = 5;
		}
		%this.stat2 = "";
	}
}


//Status effects from thirst and hunger
function player::statusTick(%this)
{
	//echo("status tick" SPC %this.client.getplayername());
	%this.statusTicked = 1;
	if(%this.healthness != 0)
	{
		if(%this.healthness < 0 || !(%this.inpain))
			%this.addHealth(%this.healthness);
	}
	if(%this.healthnessT != 0)
	{
		if(%this.healthnessT < 0 || !(%this.inpain))
			%this.addHealth(%this.healthnessT);
	}
	if(%this.sickness != 0 && %this.isSick)
		%this.addSickness(%this.sickness);
	if(%this.sicknessT != 0 && %this.isSick)
		%this.addSickness(%this.sicknessT);
	if(%this.sick == 100)
		%this.addHealth(-1);
	if(%this.infection == 100)
		%this.addHealth(-1);
	if(%this.sickness == 0 && %this.healthness == 0 && %this.healthnessT == 0 && %this.sicknessT == 0 && %this.sick != 100 && %this.infection != 100)
	{
		%this.statusTicked = 0;
		return;
	}
	%this.statusTickSch = %this.schedule($Survive::statusTick,statusTick);
}


//Tiredness function
function player::addTired(%this, %val)
{
	%this.tired +=  %val;
	if(%this.tired >= 100)
		%this.tired = 100;
	if(%this.tired < 0)
		%this.tired = 0;
	%vign = (100 - %this.tired) / 100;
	commandToClient(%this.client,'setVignette',1,%vign SPC %vign SPC %vign SPC %vign);
	if(%this.ispassedout && !%this.sleeping)
		%this.enterSleep();
	if(%this.tired >= 75)
	{
		if(%this.tiredLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6I feel all rested up!");
			%this.tiredLevel = 3;
		}
		%this.stat3 = "";
	}
	else if(%this.tired >= 50)
	{
		if(%this.tiredLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6I feel kind of tired.");
			%this.tiredLevel = 2;
		}
		%this.stat3 = "";
	}
	else if(%this.tired >= 25)
	{
		if(%this.tiredLevel != 1)
		{
	//		messageClient(%this.client,'',"\c6I feel very tired, I should sleep soon...");
			%this.tiredLevel = 1;
		}
		%this.stat3 = "<bitmap:add-ons/gamemode_wr/m_ti.png>";
	}
	else if(%this.tired == 0)
	{
		%this.enterSleep();
		%this.ispassedout = 1;
		%this.stat3 = "<bitmap:add-ons/gamemode_wr/m_ti.png>";
	}
}


//Anxiety jitter thingy
function player::jitter(%this,%in)
{
	%db = "jitter" @ %in @ "Projectile";
	%ex = new projectile()
	{
		datablock = %db;
		initialposition = %this.getPosition();
	};
	//%ex.explode();
}


//Sickness functions
function player::rollForSickness(%this)
{
	if(%this.isSick)
		return;
	if(getRandom(1,100) < $Survive::sicknessChance)
	{
    	%this.isSick = 1;
        %this.startSickTick();
    }
    if(!%this.isSick && %this.isSubmerged)
    	%this.schedule($Survive::sicknessReroll,rollForSickness);
}

function player::addSickness(%this, %val)
{
	//echo("sickness" SPC %this.client.getplayername() SPC %val);
	%this.sick += %val;
    if(%this.sick <= 0)
    {
    	%this.sick = 0;
        %this.isSick = 0;
		%this.setInpain(0);
		return;
    }
	%this.setInPain(1);
    if(%this.sick >= 100)
	{
    	%this.sick = 100;
		if(!%this.statusTicked)
			%this.statusTick();
		if(%this.sickLevel != 5)
		{
	//		messageClient(%this.client,'',"\c6I'm dying. :c");
			%this.sickLevel = 5;
		}
		%this.stat7 = "<bitmap:add-ons/gamemode_wr/m_s.png>";
	}
	else if(%this.sick > 75)
	{
		if(%this.sickLevel != 4)
		{
	//		messageClient(%this.client,'',"\c6I'm REALLY sick.");
			%this.sickLevel = 4;
		}
		%this.stat7 = "<bitmap:add-ons/gamemode_wr/m_s.png>";
	}
	else if(%this.sick > 50)
	{
		if(%this.sickLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6I think I'm sick.");
			%this.sickLevel = 3;
		}
		%this.stat7 = "<bitmap:add-ons/gamemode_wr/m_s.png>";
	}
	else if(%this.sick > 25)
	{
		if(%this.sickLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6*cough*");
			%this.sickLevel = 2;
		}
		%this.stat7 = "";
	}
	else if(%this.sick < 25)
	{
		if(%this.sickLevel != 1)
		{
	//		messageClient(%this.client,'',"\c6I feel healthy.");
			%this.sickLevel = 1;
		}
		%this.stat7 = "";
	}
}

function player::startSickTick(%this)
{
	cancel(%this.sickSch);
	if(!%this.issick)
		return;
	%this.addSickness(1);
    %this.sickSch = %this.schedule($Survive::sickTick,startSickTick);
}

//Infection stuff
function player::startInfection(%this)
{
	%this.setInpain(1);
	%this.infection++;
	if(%this.infection >= 100)
	{
    	%this.infection = 100;
		if(!%this.statusTicked)
			%this.statusTick();
		if(%this.infectionLevel != 5)
		{
	//		messageClient(%this.client,'',"\c6I'm dying. :c");
			%this.infectionLevel = 5;
		}
		%this.stat8 = "<bitmap:add-ons/gamemode_wr/m_i.png>";
	}
	else if(%this.infection > 75)
	{
		if(%this.infectionLevel != 4)
		{
	//		messageClient(%this.client,'',"\c6I'm REALLY sick.");
			%this.infectionLevel = 4;
		}
		%this.stat8 = "<bitmap:add-ons/gamemode_wr/m_i.png>";
	}
	else if(%this.infection > 50)
	{
		if(%this.infectionLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6I think I'm sick.");
			%this.infectionLevel = 3;
		}
		%this.stat8 = "<bitmap:add-ons/gamemode_wr/m_i.png>";
	}
	else if(%this.infection > 25)
	{
		if(%this.infectionLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6*cough*");
			%this.infectionLevel = 2;
		}
		%this.stat8 = "";
	}
	%this.schedule($Survive::SickTick,startInfection);
}
	

//Wetness stuffs
function player::addWet(%this, %val)
{
	%this.wet += %val;
    if(%this.wet >= 100)
	{
    	%this.wet = 100;
		%this.rollForSickness();
	}
    if(%this.wet < 0)
    	%this.wet = 0;
	if(%this.wet > 75)
	{
		if(%this.wetLevel != 4)
		{
	//		messageClient(%this.client,'',"\c6I'm soaked!");
			%this.wetLevel = 4;
		}
		%this.stat4 = "<bitmap:add-ons/gamemode_wr/m_w.png>";
	}
	else if(%this.wet > 50)
	{
		if(%this.wetLevel != 3)
		{
	//		messageClient(%this.client,'',"\c6I'm pretty wet...");
			%this.wetLevel = 3;
		}
		%this.stat4 = "";
	}
	else if(%this.wet > 25)
	{
		if(%this.wetLevel != 2)
		{
	//		messageClient(%this.client,'',"\c6I'm a little wet.");
			%this.wetLevel = 2;
		}
		%this.stat4 = "";
	}
	else if(%this.wet < 25)
	{
		if(%this.wetLevel != 1)
		{
	//		messageClient(%this.client,'',"\c6I'm dry.");
			%this.wetLevel = 1;
		}
		%this.stat4 = "";
	}
}

function player::decreaseWet(%this)
{
	%this.addWet(-1);
    %this.deWetSch = %this.schedule($Survive::wetTick,decreaseWet);
}


//Anxiety stuff
function player::deAnx(%this)
{
	//echo(%this.client.getplayername() SPC %this.anxiety);
	if(%this.inpain)
	{
		%this.deAnx = %this.schedule($Survive::anxietyTick,deAnx);
		%this.jitter(1);
		return;
	}
	if(%this.anxiety <= 0)
	{
		%this.anxiety = 0;
		%this.decreasingAnx = 0;
		return;
	}
	%this.decreasingAnx = 1;
	if(%this.anxiety > 0)
		%this.anxiety--;
	if(%this.anxiety < 25)
	{
	//	if(%this.deanxLevel != 0)
	//		messageClient(%this.client,'',"\c6Ok, everything's going to be fine.");
		%this.deanxLevel = 0;
		%this.stat5 = "";
	}		
	else if(%this.anxiety < 50)
	{
	//	if(%this.deanxLevel != 1)
	//		messageClient(%this.client,'',"\c6I'm still shaken up...");
		%this.deanxLevel = 1;
		%this.jitter(1);
		%this.stat5 = "<bitmap:add-ons/gamemode_wr/m_so.png>";
	}
	else if(%this.anxiety < 75)
	{
	//	if(%this.deanxLevel != 2)
	//		messageClient(%this.client,'',"\c6I think they're gone...");
		%this.deanxLevel = 2;
		%this.jitter(1);
		%this.stat5 = "<bitmap:add-ons/gamemode_wr/m_so.png>";
	}
	else if(%this.anxiety > 75)
	{
	//	if(%this.deanxLevel != 3)
	//		messageClient(%this.client,'',"\c6*whimper*");
		%this.deanxLevel = 3;
		%this.jitter(1);
		%this.stat5 = "<bitmap:add-ons/gamemode_wr/m_tr.png>";
	}
	%this.deAnx = %this.schedule($Survive::anxietyTick,deAnx);
}

function player::tryDeAnx(%this)
{
	if(!%this.decreasingAnx)
		%this.deAnx();
}

function player::startAnxiety(%this, %type, %zombie, %val)
{
	if((!isObject(%zombie) || %zombie.hFollowing != %this) && %type == 0)
	{
		%this.followingZombies = strReplace(%this.followingZombies, %zombie, "");
		%this.deAnx = %this.schedule(5000,trydeAnx);
		return;
	}
	cancel(%this.deAnx);
	%this.decreasingAnx = 0;
	//echo(%this.client.getplayername() SPC %this.anxiety);
	if(!(%this.anxiety >= 100) && %type == 0)
		%this.anxiety++;
	else if(%this.anxiety <= 100)
		%this.anxiety += %val;
	if(%this.anxiety > 100)
		%this.anxiety = 100;
	if(%this.anxiety > 99)
	{
	//	if(%this.anxLevel != 3)
	//		messageClient(%this.client,'',"\c6AHHHHHHHH!");
		%this.anxLevel = 3;
		%this.jitter(3);
		%this.stat5 = "<bitmap:add-ons/gamemode_wr/m_tr.png>";
	}
	else if(%this.anxiety > 66)
	{
	//	if(%this.anxLevel != 2)
	//		messageClient(%this.client,'',"\c6I'm freaking out here!");
		%this.anxLevel = 2;
		%this.jitter(2);
		%this.stat5 = "<bitmap:add-ons/gamemode_wr/m_so.png>";
	}
	else if(%this.anxiety > 33)
	{
	//	if(%this.anxLevel != 1)
	//		messageClient(%this.client,'',"\c6I feel nervous...");
		%this.anxLevel = 1;
		%this.jitter(1);
		%this.stat5 = "";
	}
	if(%type == 1)
	{
		%this.deAnx = %this.schedule(5000,tryDeAnx);
		return;
	}
	%this.schedule($Survive::anxietyTick, startAnxiety, 0, %zombie);
}


//player inpain set function
function player::setInPain(%this,%val)
{
	%this.inpain = %val;
	if(%val)
		%this.stat6 = "<bitmap:add-ons/gamemode_wr/m_ip.png>";
	else
		%this.stat6 = "";
}

//Bottomprint statuses
function player::displayStats(%this)
{
	cancel(%this.statSch);
	%c = %this.client;
	if(!isObject(%c))
		return;
	%c.centerPrint("<just:right>" @ %this.stat1 @ %this.stat2 @ %this.stat3 @ %this.stat4 @ %this.stat5 @ %this.stat6 @ %this.stat7 @ %this.stat8,5,0);
	
	%tired = mFloatLength(%this.tired/10,0);
	%hp = mFloatLength(%this.health/10,0);
	%hunger = mFloatLength(%this.hunger/10,0);
	%thirst = mFloatLength(%this.thirst/10,0);
	%wet = mFloatLength(%this.wet/10,0);
	if(%this.infection > %this.sick)
		%sick = mFloatLength(%this.infection/10,0);
	else
		%sick = mFloatLength(%this.sick/10,0);
	
	//Debug display
	//%c.bottomPrint("<color:ffffff>" @ %c.player.identity @ "<just:right>Ti" @ %this.tired SPC "HP" @ %this.health SPC "Ax" @ %this.anxiety SPC "Hu" @ %this.hunger SPC "Si" @ %this.sick SPC "Th" @ %this.thirst SPC "We" @ %this.wet SPC "In" @ %this.infection, 5);
	%c.bottomPrint("<color:ffffff>" @ %c.player.identity @ "<just:right>Stamina:" @ %tired SPC "HP:" @ %hp SPC "Hunger:" @ %hunger SPC "Thirst:" @ %thirst SPC "Wetness:" @ %wet SPC "Sickness:" @ %sick, 5);
	%this.statSch = %this.schedule(4900,displayStats);
}

//package for:
// -saving moodles
// -enter/leave water detection
// -setting inPain status
// -infection chance from zombies
// -terror from being followed by zombies

package SurviveMoodle
{
	function armor::onDamage(%this, %obj, %amount)
	{
		%obj.health -= %amount;
		if(%amount > 15)
		{
			%obj.setinpain(1);
			%obj.startAnxiety(1,"",5);
			cancel(%obj.painSch); 
			%obj.painSch = %obj.schedule($survive::paintime,setinpain,0);
		}
		else if(%amount > 5)
		{
			%obj.setinpain(1);
			%obj.startAnxiety(1,"",5);
			cancel(%obj.painSch);
			%obj.painSch = %obj.schedule($survive::smallpaintime,setinpain,0);
		}
		%obj.displayStats();
		Parent::onDamage(%this,%obj,%amount);
	}
	function GameConnection::spawnPlayer(%this)
	{
		parent::spawnPlayer(%this);
		%this.player.tired = 100;
		%this.player.health = 100;
		%this.player.anxiety = 0;
		%this.player.hunger = 100;
		%this.player.sick = 0;
		%this.player.thirst = 100;
		%this.player.wet = 0;
		%this.player.infection = 0;
		%this.player.isSick = 0;
		%this.player.isInfected = 0;
		%this.player.isPassedOut = 0;
		%this.player.sicklevel = 1;
		%this.player.infectionlevel = 1;
		%this.player.foodLevel = 5;
		%this.player.thirstLevel = 5;
		%this.player.tiredLevel = 3;
		%this.player.setHealth(%this.player.health);
		%this.player.displayStats();
	}
	function armor::onEnterLiquid(%this, %obj, %arg1, %arg2)
	{
		parent::onEnterLiquid(%this, %obj, %arg1, %arg2);
		%obj.isSubmerged = 1;
		%obj.addWet(100);
		cancel(%obj.deWetSch);
	}
	function armor::onLeaveLiquid(%this, %obj, %arg1)
	{
		parent::onLeaveLiquid(%this, %obj, %arg1);
		%obj.isSubmerged = 0;
		%obj.deWetSch = %obj.schedule(5000,decreaseWet);
	}
	function AIPlayer::hFollowPlayer( %obj, %targ, %inHoleLoop, %skipAlert )
	{
		parent::hFollowPlayer( %obj, %targ, %inHoleLoop, %skipAlert );
		if(%targ.getClassname() $="Player")
		{
			if(strPos(%targ.followingZombies,%obj) == -1)
			{
				%targ.followingZombies = %targ.followingZombies @ %obj; 
				%targ.startAnxiety(0,%obj,0);
			}
		}
	}
	function AIPlayer::hMeleeAttack( %obj, %col )
	{
		parent::hMeleeAttack( %obj, %col );
		if(%col.getType() & $TypeMasks::PlayerObjectType)
		{
			if(getRandom(1,100) < $Survive::infectionChance)
				%col.startInfection();
		}
	}
};
activatePackage(SurviveMoodle);
globalHungerTick();
globalThirstTick();
globalTiredTick();