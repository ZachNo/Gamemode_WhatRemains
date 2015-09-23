//By ZSNO

datablock audioProfile(zombie1Sound)
{
	filename = "./sounds/zombieSound1.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie2Sound)
{
	filename = "./sounds/zombieSound2.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie3Sound)
{
	filename = "./sounds/zombieSound3.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie4Sound)
{
	filename = "./sounds/zombieSound4.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie5Sound)
{
	filename = "./sounds/zombieSound5.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie6Sound)
{
	filename = "./sounds/zombieSound6.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie7Sound)
{
	filename = "./sounds/zombieSound7.wav";
	description = audioFar3d;
	preload = 1;
};
datablock audioProfile(zombie8Sound)
{
	filename = "./sounds/zombieSound8.wav";
	description = audioFar3d;
	preload = 1;
};

$Survive::botSpawnTimeout = 420000;

datablock fxDTSBrickData (BrickZombieWR_HoleSpawnData)
{
	brickFile = "Add-ons/Bot_Hole/4xSpawn.blb";
	category = "Special";
	subCategory = "Holes";
	uiName = "WRZombie Hole";
	iconName = "Add-Ons/Bot_Zombie/icon_zombie";

	bricktype = 2;
	cancover = 0;
	orientationfix = 1;
	indestructable = 1;

	isBotHole = 1;
	holeBot = "WRZombieHoleBot";
};

datablock PlayerData(WRZombieHoleBot : ZombieHoleBot)
{
	uiName = "";
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	maxItems   = 0;
	maxWeapons = 0;
	maxTools = 0;
	runforce = 100 * 90;
	maxForwardSpeed = 6;
	maxBackwardSpeed = 4;
	maxSideSpeed = 6;
	attackpower = 10;
	rideable = false;
	canRide = true;
	maxdamage = 50;//Health
	jumpSound = "";
	//Hole Attributes
	isHoleBot = 1;
	//Spawning option
	hSpawnTooClose = 1;//Doesn't spawn when player is too close and can see it
	  hSpawnTCRange = 8;//above range, set in brick units
	hSpawnClose = 1;//Only spawn when close to a player, can be used with above function as long as hSCRange is higher than hSpawnTCRange
	  hSpawnCRange = 256;//above range, set in brick units
	hType = zombie; //Enemy,Friendly, Neutral
	  hNeutralAttackChance = 100;
	//can have unique types, nazis will attack zombies but nazis will not attack other bots labeled nazi
	hName = "Zombie";//cannot contain spaces
	hTickRate = 3000;
	//Wander Options
	hWander = 1;//Enables random walking
	  hSmoothWander = 1;
	  hReturnToSpawn = 1;//Returns to spawn when too far
	  hSpawnDist = 32;//Defines the distance bot can travel away from spawnbrick
	//Searching options
	hSearch = 1;//Search for Players
	  hSearchRadius = 256;//in brick units
	  hSight = 1;//Require bot to see player before pursuing
	  hStrafe = 1;//Randomly strafe while following player
	hSearchFOV = 1;//if enabled disables normal hSearch
	  hFOVRadius = 32;//max 10
	  hAlertOtherBots = 1;//Alerts other bots when he sees a player, or gets attacked
	//Attack Options
	hMelee = 1;//Melee
	  hAttackDamage = 30;//15;//Melee Damage
	hShoot = 0;
	  hWep = "gunImage";
	  hShootTimes = 4;//Number of times the bot will shoot between each tick
	  hMaxShootRange = 30;//The range in which the bot will shoot the player
	  hAvoidCloseRange = 1;//
		hTooCloseRange = 7;//in brick units
	//Misc options
	hAvoidObstacles = 1;
	hSuperStacker = 1;
	hSpazJump = 1;//Makes bot jump when the user their following is higher than them
	hAFKOmeter = 1;//Determines how often the bot will wander or do other idle actions, higher it is the less often he does things
	hIdle = 1;// Enables use of idle actions, actions which are done when the bot is not doing anything else
	  hIdleAnimation = 1;//Plays random animations/emotes, sit, click, love/hate/etc
	  hIdleLookAtOthers = 1;//Randomly looks at other players/bots when not doing anything else
	    hIdleSpam = 0;//Makes them spam click and spam hammer/spraycan
	  hSpasticLook = 1;//Makes them look around their environment a bit more.
	hEmote = 1;
	hMaxMoveSpeed = 0.75;
};

function AIPlayer::zombieMoan(%this)
{
	%soundi = getRandom(1,8);
	%sound = "zombie" @ %soundi @ "Sound";
	%this.playAudio(0,%sound);
	%this.schedule(7000 + getRandom(0,100),zombieMoan);
}

function AIPlayer::wrZombie(%obj)
{
	%obj.playthread(1,"ArmReadyBoth");
	%obj.hDefaultThread = "ArmReadyBoth";
	%skinColor = "0.541 0.698 0.553 1";
	%handColor = %skinColor;
	%hatColor = getRandomBotColor();
	%packColor = getRandomBotColor();
	%shirtColor = getRandomBotColor();
	%accentColor = getRandomBotColor();
	%pantsColor = getRandomBotPantsColor( %shirtColor );
	%shoeColor = %pantsColor;
	%hat = 0;
	%accent = 0;
	%pack = 0;
	%pack2 = 0;
	%decal = "AAA-None";
	%face = "asciiTerror";
	%decal[ %nDecal = 0 ] = "Mod-Army";
	%decal[ %nDecal++ ] = "Mod-Police";
	%decal[ %nDecal++ ] = "Mod-Suit";
	%decal[ %nDecal++ ] = "Meme-Mongler";
	%decal[ %nDecal++ ] = "Mod-DareDevil";
	%decal[ %nDecal++ ] = "Mod-Pilot";
	%decal[ %nDecal++ ] = "Mod-Prisoner";
	%decal[ %nDecal++ ] = "AAA-None";
	%decal = %decal[ getRandom( 0, %nDecal ) ];
	%hat[ %nHat = 0 ] = 4;
	%hat[ %nHat++ ] = 6;
	%hat[ %nHat++ ] = 7;
	%hat[ %nHat++ ] = 0;
	%hat = %hat[ getRandom( 0, %nHat ) ];
	%obj.accentColor = %accentColor;
	%obj.accent =  %accent;
	%obj.hatColor = %hatColor;
	%obj.hat = %hat;
	%obj.headColor = %skinColor;
	%obj.faceName = %face;
	%obj.chest =  "0";
	%obj.decalName = %decal;
	%obj.chestColor = %shirtColor;
	%obj.pack =  %pack;
	%obj.packColor =  %packColor;
	%obj.secondPack =  %pack2;
	%obj.secondPackColor =  "0 0.435 0.831 1";
	%obj.larm =  "0";
	%obj.larmColor = %shirtColor;
	%obj.lhand =  "0";
	%obj.lhandColor = %handColor;
	%obj.rarm =  "0";
	%obj.rarmColor = %shirtColor;
	%obj.rhandColor = %handColor;
	%obj.rhand =  "0";
	%obj.hip =  "0";
	%obj.hipColor = %pantsColor;
	%obj.lleg =  "0";
	%obj.llegColor = %shoeColor;
	%obj.rleg =  "0";
	%obj.rlegColor = %shoeColor;
	GameConnection::ApplyBodyParts(%obj);
	GameConnection::ApplyBodyColors(%obj);
	%obj.isInfected = 1;
	%obj.zombieMoan();
}

function fxDTSBrick::setRespawnTime(%this, %time)
{
	%this.itemRespawnTime = %time;
}

package SurviveBotSpawning
{
	function fxDTSBrick::onPlant(%this)
    {
    	parent::onPlant(%this);
        if(%this.getDatablock() == BrickZombieWR_HoleSpawnData.getID())
        {
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
			%this.schedule(100,setRespawnTime,300000);
        }
	}
	function fxDTSBrick::onLoadPlant(%this)
    {
    	parent::onLoadPlant(%this);
        if(%this.getDatablock() == BrickZombieWR_HoleSpawnData.getID())
        {
			%this.setRendering(0);
			%this.setColliding(0);
			%this.setRaycasting(0);
			%this.schedule(100,setRespawnTime,300000);
        }
	}
	function WRZombieHoleBot::onAdd(%this,%obj)
	{
		parent::onAdd(%this,%obj);
		%obj.wrZombie();
	}
};
activatePackage(SurviveBotSpawning);