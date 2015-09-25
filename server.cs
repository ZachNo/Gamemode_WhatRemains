//What Remains Gamemode
//Basically just a bunch of things I made packaged into one

//TODO:
//Test combat logging again
//Test armor staying on

//LOW PRIORITY:
//- maybe modify zombie behavoir

//Set brick limit to higher than normal
schedule(1,0,eval,"$Pref::Server::BrickLimit = 300000;");
//schedule(1,0,eval,"$Pref::Server::GhostLimit = 1048576;");

//Set other variables for other add-ons
$BKT::CH = 0;
$BD::Enabled = 1;
$BD::Time = 0;
$avatarLockToggle = 1;
$ClickPush::Status = 1;
$SMMBodies::Remove = 0;
$SMMBodies::CanCarry = 0;
$VGM::Enabled = 1;

//Modded Over the shoulder player
datablock PlayerData(PlayerOTSNoJet : PlayerStandardArmor)
{
	camerahorizontaloffset = 1;
	cameraverticaloffset = 1;
	cameraMaxDist = 1;
	minJetEnergy = 0;
	jetEnergyDrain = 0;
	canJet = 0;
	//thirdPersonOnly = 1;

	uiName = "Over the Shoulder Player";
};

//Spawn Protection
$spawnProtectionTime = 20;
$enableSpawnProtection = 1;
package spawnProtection
{
	function GameConnection::spawnPlayer(%this)
	{
		parent::spawnPlayer(%this);
		%player = %this.player;
		%player.setShapeNameDistance(0);
		if($enableSpawnProtection)
		{
			%player.spawnProtected = true;
			%player.schedule($spawnProtectionTime, noProtection);
		}
		%this.schedule(10,checkFist);
		schedule(10,0,checkArmor,%this);
	}
	function Player::damage(%moo, %moo2, %moo3, %moo4, %moo5, %moo6)
	{
		if(%moo.spawnProtected)
			return;
		Parent::damage(%moo, %moo2, %moo3, %moo4, %moo5, %moo6);
	}
	function WheeledVehicleData::Damage(%data, %obj, %sourceObj, %position, %amount, %damageType, %momentum)
	{
		if(%obj.isCorpseVehicle)
		{
			%obj.corpse.delete();
			%obj.delete();
			return;
		}
		parent::damage(%data, %obj, %sourceObj, %position, %amount, %damageType, %momentum);
	}
	function gameConnection::onDeath(%this,%obj,%killer,%type,%area)
	{
		%client.quantity["9mmrounds"] = 0;
		%client.quantity["45Caliber"] = 0;
		%client.quantity["57rounds"] = 0;
		%client.quantity["556rounds"] = 0;
		%client.quantity["762mmrounds"] = 0;
		%client.quantity["akrounds"] = 0;
		%client.quantity["shotgunrounds"] = 0;
		parent::onDeath(%this,%obj,%killer,%type,%area);
	}
	
};
activatePackage(spawnProtection);
function Player::noProtection(%this)
{
	%this.spawnProtected = false;
}

//Check player's inventory to see if they need the fist weapon or not
function GameConnection::checkFist(%this)
{
	%player = %this.player;
	%player.setHealth(%player.health);
	if(!isObject(%player))
		return;
	//check to see if player has fists/backpack
	%needFistItem = 1;
	%needBackpack = 1;
	for(%i = 0; %i < 5; %i++)
	{
		if(%player.tool[%i] == fistItem.getID())
			%needFistItem = 0;
		if(%player.tool[%i] == ISBackPackItem.getID())
			%needBackpack = 0;
	}
	for(%i = 0; %i < 5; %i++)
	{
		if(%player.tool[%i] != 0)
			continue;
		if(%needFistItem)
		{
			%player.tool[%i] = fistItem.getID();
			messageClient(%this, 'MsgItemPickup', '', %i, fistItem.getID());
			%player.weaponCount++;
			%needFistItem = 0;
		}
		else if(%needBackpack)
		{
			%player.tool[%i] = ISBackPackItem.getID();
			messageClient(%this, 'MsgItemPickup', '', %i, ISBackPackItem.getID());
			%player.weaponCount++;
			%needBackpack = 0;
		}
	}
}

deactivatepackage(AmmoGuns2);
package AmmoGuns3
{
	function Player::pickup(%this,%item)
	{
		%data = %item.dataBlock;
		%ammo = %item.weaponAmmoLoaded;
		%val = Parent::pickup(%this,%item);
		if(%val == 1 && %data.maxAmmo > 0 && isObject(%this.client))
		{
			%slot = -1;
			for(%i=0;%i<%this.dataBlock.maxTools;%i++)
			{
				if(isObject(%this.tool[%i]) && %this.tool[%i].getID() == %data.getID() && %this.toolAmmo[%i] $= "")
				{
					%slot = %i;
					break;
				}
			}
			
			if(%slot == -1)
				return %val;
			
			if(%ammo $= "")
			{
				%this.toolAmmo[%slot] = 0;//%data.maxAmmo;
			}
			else
			{
				%this.toolAmmo[%slot] = %ammo;
			}
		}
		return %val;
	}
	
	function ItemData::onAdd(%this,%obj)
	{
		if($weaponAmmoLoaded !$= "")
		{
			%obj.weaponAmmoLoaded = $weaponAmmoLoaded;
			$weaponAmmoLoaded = "";
		}
		Parent::onAdd(%this,%obj);
	}
	
	//Check if the gun needs to reload. Use this to trigger state changes.
	function WeaponImage::onLoadCheck(%this,%obj,%slot)
	{
		if(%obj.toolAmmo[%obj.currTool] <= 0 && %this.item.maxAmmo > 0 && %obj.getState() !$= "Dead")
			%obj.setImageAmmo(%slot,0);
		else
			%obj.setImageAmmo(%slot,1);
	}
	
	//Use this state in single-ammo reload weapons e.g. Shotgun, Scattergun
	function WeaponImage::onReloadCheck(%this,%obj,%slot)
	{
		if(%obj.toolAmmo[%obj.currTool] < %this.item.maxAmmo && %this.item.maxAmmo > 0 && %obj.getState() !$= "Dead")
			%obj.setImageAmmo(%slot,0);
		else
			%obj.setImageAmmo(%slot,1);
	}
	
	//Example, you may wish to have weapons load all at once
	function WeaponImage::onReloaded(%this,%obj,%slot)
	{
		%obj.toolAmmo[%obj.currTool]++;
	}
	
	function servercmdLight(%client)
	{
		if(isObject(%client.player) && isObject(%client.player.getMountedImage(0)))
		{
			%p = %client.player;
			%im = %p.getMountedImage(0);
			if(%im.item.maxAmmo > 0 && %im.item.canReload == 1 && %p.toolAmmo[%p.currTool] < %im.item.maxAmmo)
			{
				if(%p.getImageState(0) $= "Ready")
					%p.setImageAmmo(0,0);
				return;
			}
		}
		
		Parent::servercmdLight(%client);
	}
};
activatepackage(AmmoGuns3);
RegisterPersistenceVar("quantity9MMrounds",false,"");
RegisterPersistenceVar("quantity45caliber",false,"");
RegisterPersistenceVar("quantity500rounds",false,"");
RegisterPersistenceVar("quantityshotgunrounds",false,"");
RegisterPersistenceVar("quantity556rounds",false,"");
RegisterPersistenceVar("quantity762mmrounds",false,"");
RegisterPersistenceVar("quantity338rounds",false,"");
RegisterPersistenceVar("quantity50calrounds",false,"");
RegisterPersistenceVar("quantity57rounds",false,"");
RegisterPersistenceVar("quantityakrounds",false,"");
RegisterPersistenceVar("quantity762mmrounds",false,"");
RegisterPersistenceVar("quantity44Prounds",false,"");
RegisterPersistenceVar("armor", false, "");
RegisterPersistenceVar("armored", false, "");
RegisterPersistenceVar("armorDivider", false, "");

//Overrides the connect, leave, and death chat messages
package override_minigameMessage
{
	function messageClient(%client, %type, %msg, %a, %b, %c, %d, %e)
	{
		//echo(%client SPC "|" SPC %type SPC "|" SPC %msg SPC "|" SPC %a SPC "|" SPC %b SPC "|" SPC %c SPC "|" SPC %d SPC "|" SPC %e);
	
		if(%msg $= "<color:FFFFFF>This server runs <color:F00F00>Headshot Mod<color:FFFFFF> V1")
			return;
	
		%tag = getTaggedString(%type);
		%msgTag = getTaggedString(%msg);
		
		if(%tag $= "MsgClientInYourMiniGame" || %tag $= "MsgClientJoin" || %tag $= "MsgYourDeath" || %tag $= "MsgClientKilled") //type the minigame message uses
			return;
		if(%msgTag $= "\c1%1 connected." || %msgTag $= "\c1%1 spawned." || %msgTag $= "\c1%1 has left the game.")
			return;
		
		if(%tag $= "MsgItemPickup" && isObject(%client.player))
		{
			if(%b == 0 && %client.player.toolammo[%a] > 0)
			{
				%wepID = %client.player.lasttool[%a].getID();
				if(%wepID == G18FAOItem.getID() || %wepID == HKMP5KItem.getID() || %wepID == PP90M1Item.getID())
				{
					%client.quantity["9mmrounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["9mmrounds"] > 30*10)
					{
						%client.quantity["9mmrounds"] = 30*10;
					}
				}
				
				if(%wepID == AIAWMItem.getID() || %wepID == CM1911A1Item.getID() || %wepID == HKUSP45Item.getID() || %wepID == MMSItem.getID())
				{
					%client.quantity["45Caliber"] += %client.player.toolAmmo[%a];
					if(%client.quantity["45Caliber"] > 30*6)
					{
						%client.quantity["45Caliber"] = 30*6;
					}
				}
				
				if(%wepID == FNP90USGItem.getID())
				{
					%client.quantity["57rounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["57rounds"] > 50*5)
					{
						%client.quantity["57rounds"] = 50*5;
					}
				}
				
				if(%wepID == HKL86Item.getID() || %wepID == M4ComItem.getID() || %wepID == DARItem.getID() || %wepID == MPDRItem.getID())
				{
					%client.quantity["556rounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["556rounds"] > 30*15)
					{
						%client.quantity["556rounds"] = 30*15;
					}
				}
				
				if(%wepID == NAVSEA14Item.getID() || %wepID == JNG90Item.getID() || %wepID == FNFALDMRItem.getID() || %wepID == HK21Item.getID())
				{
					%client.quantity["762mmrounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["762mmrounds"] > 30*10)
					{
						%client.quantity["762mmrounds"] = 30*10;
					}
				}
				
				if(%wepID == SUAK47Item.getID() || %wepID == SUAKMItem.getID())
				{
					%client.quantity["akrounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["akrounds"] > 30*10)
					{
						%client.quantity["akrounds"] = 30*10;
					}
				}
				
				if(%wepID == Moss500Item.getID() || %wepID == FSPAS12Item.getID())
				{
					%client.quantity["shotgunrounds"] += %client.player.toolAmmo[%a];
					if(%client.quantity["shotgunrounds"] > 6*14)
					{
						%client.quantity["shotgunrounds"] = 6*14;
					}
				}
				%client.player.toolAmmo[%a] = 0;
			}
			%client.player.lastTool[%a] = %b;
		}
		
		return parent::messageClient(%client, %type, %msg, %a, %b, %c, %d, %e);
	}
	
	//Overwrite normal load persistance function to load EXACT position
   function GameConnection::applyPersistence(%client, %gotPlayer, %gotCamera)
   {
      %camera = %client.camera;
      %player = %client.player;

      echo("Applying persistence" SPC %gotPlayer SPC %gotCamera);

      if(!%gotPlayer && %gotCamera)
      {
         %client.setControlObject(%camera);      
         %camera.setOrbitPointMode(%camera.orbitPoint, %camera.orbitDistance);

         if(isObject(%player))
            %player.delete();
      }
      else if(%gotPlayer)
      {
         //tell client about inventory
         %toolCount = %player.getDataBlock().maxTools;
         for(%i = 0; %i < %toolCount; %i++)
         {
            messageClient(%client, 'MsgItemPickup', "", %i, %player.tool[%i], 1); //the last 1 = silent
         }

         //make player use the last tool they had out
         if(%player.currTool >= 0 && %player.currTool < %toolCount)
         {
            commandToClient(%client, 'setActiveTool', %player.currTool);
         }
         else if(%client.currInv >= 0 && %client.currInv < %player.getDataBlock().maxItems)
         {
            commandToClient(%client, 'SetActiveBrick', %client.currInv);
         }
      }
   
   }
	function armorKevlarImage::onFire( %this, %obj, %slot )
	{
		%data = %this.item;

		if( !%data.isArmor )
		{
			return;
		}

		%obj.unmountimage( 3 );
		%obj.mountimage( armorKevlarEquippedImage, 3 );
		serverPlay3d( cssEquipSound, %obj.getHackPosition() );

		%obj.armor = %data.health;
		%obj.armorDivider = %data.divider;
		%obj.armored = true;

		messageClient( %obj.client, 'MsgItemPickup', '', %obj.currTool, 0 );
		%obj.tool[ %obj.currTool ] = 0;
		serverCmdUnUseTool( %obj.client );
	}
	function Player::Damage( %this, %obj, %pos, %damage, %damageType )
	{
		if( %this.armored )
		{
			%this.armor -= %damage * 0.5;
			%damage = %damage * %this.armorDivider;
			commandToClient( %this.client, 'bottomprint', "\c6Armor\c3:" SPC MCeil( %this.armor ) @ "\c6.", 3, true );
			if( %this.armor <= 0 )
			{
				%this.unmountImage( 3 );
				commandToClient( %this.client, 'bottomprint', "\c6Armor\c3: 0\c6.", 3, true );
				serverPlay3d( helmetHitSound, %this.getHackPosition() );
			}
			else
				serverPlay3d( kevlarHitSound @ getRandom( 1, 3 ), %this.getHackPosition() ); // GetRandom in there randomises between the 3 different hitsounds.
		}
		parent::Damage( %this, %obj, %pos, %damage, %damageType );
	}
	
	//=================================PISTOL AMMO===============
	function PistolAmmostaticImage::onMount(%this,%obj,%slot)
	{
		%obj.playThread(0,armReadyBoth);
	}
	function PistolAmmostaticImage::onUnMount(%this,%obj,%slot)
	{
		%obj.playThread(0,root);
	}
	function PistolAmmostaticImage::onFire(%this,%obj,%slot)
	{
		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.client.quantity["9mmrounds"] += 30;
				if(%obj.client.quantity["9mmrounds"] > 30*10)
				{
					%obj.client.quantity["9mmrounds"] = 30*10;
				}
				%obj.client.quantity["500rounds"] += 5;
				if(%obj.client.quantity["500rounds"] > 5*5)
				{
					%obj.client.quantity["500rounds"] = 5*5;
				}
				%obj.client.quantity["45Caliber"] += 30;
				if(%obj.client.quantity["45Caliber"] > 30*6)
				{
					%obj.client.quantity["45Caliber"] = 30*6;
				}
				%obj.client.quantity["40SWrounds"] += 30;
				if(%obj.client.quantity["40SWrounds"] > 30*8)
				{
					%obj.client.quantity["40SWrounds"] = 30*8;
				}
				%obj.client.quantity["46mmrounds"] += 40;
				if(%obj.client.quantity["46mmrounds"] > 20*6)
				{
					%obj.client.quantity["46mmrounds"] = 20*6;
				}
				%obj.client.quantity["44Prounds"] += 12;
				if(%obj.client.quantity["44Prounds"] > 12*4)
				{
					%obj.client.quantity["44Prounds"] = 12*4;
				}
				%obj.client.quantity["57rounds"] += 50;
				if(%obj.client.quantity["57rounds"] > 50*5)
				{
					%obj.client.quantity["57rounds"] = 50*5;
				}
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}	
	}
	//=================================RIFLE AMMO===============
	function RifleAmmostaticImage::onMount(%this,%obj,%slot)
	{
		%obj.playThread(0,armReadyBoth);
	}
	function RifleAmmostaticImage::onUnMount(%this,%obj,%slot)
	{
		%obj.playThread(0,root);
	}

	function RifleAmmostaticImage::onFire(%this,%obj,%slot)
	{
		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.client.quantity["556rounds"] += 30;
				if(%obj.client.quantity["556rounds"] > 30*15)
				{
					%obj.client.quantity["556rounds"] = 30*15;
				}
				%obj.client.quantity["762mmrounds"] += 30;
				if(%obj.client.quantity["762mmrounds"] > 30*10)
				{
					%obj.client.quantity["762mmrounds"] = 30*10;
				}
				%obj.client.quantity["545rounds"] += 45;
				if(%obj.client.quantity["545rounds"] > 15*8)
				{
					%obj.client.quantity["545rounds"] = 15*8;
				}
				%obj.client.quantity["300AProunds"] += 20;
				if(%obj.client.quantity["300AProunds"] > 20*5)
				{
					%obj.client.quantity["300AProunds"] = 20*5;
				}
				%obj.client.quantity["akrounds"] += 30;
				if(%obj.client.quantity["akrounds"] > 30*10)
				{
					%obj.client.quantity["akrounds"] = 30*10;
				}
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}	
	}

	//=================================BIG RIFLE AMMO===============
	function SniperAmmostaticImage::onMount(%this,%obj,%slot)
	{
		%obj.playThread(0,armReadyBoth);
	}
	function SniperAmmostaticImage::onUnMount(%this,%obj,%slot)
	{
		%obj.playThread(0,root);
	}
		
	function SniperAmmostaticImage::onFire(%this,%obj,%slot)
	{
		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.client.quantity["408rounds"] += 7;
				if(%obj.client.quantity["408rounds"] > 7*3)
				{
					%obj.client.quantity["408rounds"] = 7*3;
				}
				%obj.client.quantity["50Cal"] += 10;
				if(%obj.client.quantity["50Cal"] > 5*4)
				{
					%obj.client.quantity["50Cal"] = 5*4;
				}
				%obj.client.quantity["338rounds"] += 10;
				if(%obj.client.quantity["338rounds"] > 4*7)
				{
					%obj.client.quantity["338rounds"] = 4*7;
				}
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}	
	}

	//=================================SHOTGUN AMMO===============
	function ShotgunAmmostaticImage::onMount(%this,%obj,%slot)
	{
		%obj.playThread(0,armReadyBoth);
	}
	function ShotgunAmmostaticImage::onUnMount(%this,%obj,%slot)
	{
		%obj.playThread(0,root);
	}

	function ShotgunAmmostaticImage::onFire(%this,%obj,%slot)
	{
		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.client.quantity["shotgunrounds"] += 15;
				if(%obj.client.quantity["shotgunrounds"] > 6*14)
				{
					%obj.client.quantity["shotgunrounds"] = 6*14;
				}
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}	
	}

	//=================================EXPLOSIVES AMMO===============
	function ExploAmmostaticImage::onFire(%this,%obj,%slot)
	{
		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.client.quantity["20x42mmRounds"] += 2;
				if(%obj.client.quantity["20x42mmRounds"] > 14)
				{
					%obj.client.quantity["20x42mmRounds"] = 14;
				}
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}	
	}
};
activatePackage(override_minigameMessage);
deactivatePackage(PistolAmmoStaticPackage);
deactivatePackage(RifleAmmoStaticPackage);
deactivatePackage(SniperAmmoStaticPackage);
deactivatePackage(ShotgunAmmoStaticPackage);
deactivatePackage(ExploAmmoStaticPackage);
deactivatePackage(BKTNotifier);
deactivatePackage(BKTAmmoOnSpawn);

datablock ItemData(PistolAmmostaticItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./bl_ammoPistol.dts";
	mass = 2;
	density = 1.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Ammo, Pistol Rounds";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	image = PistolAmmostaticImage;
};
datablock ShapeBaseImageData(PistolAmmostaticImage)
{
   shapeFile = "./bl_ammoPistol.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" );
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = PistolAmmostaticItem;
   ammo = " ";
   casing = gunShellDebris;
   shellExitDir        = "1.0 -1.3 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 15.0;	
   shellVelocity       = 7.0;
   melee = false;
   armReady = true;
   doColorShift = false;
   
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";
	stateName[2]                    = "Fire";
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
};
datablock ItemData(RifleAmmostaticItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./bl_ammoRifle.dts";
	mass = 2;
	density = 1.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Ammo, Rifle Rounds";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	ammocount = 30*2;
	image = RifleAmmostaticImage;
};
datablock ShapeBaseImageData(RifleAmmostaticImage)
{
   shapeFile = "./bl_ammoRifle.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" );
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = RifleAmmostaticItem;
   ammo = " ";
   casing = gunShellDebris;
   shellExitDir        = "1.0 -1.3 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 15.0;	
   shellVelocity       = 7.0;
   melee = false;
   armReady = true;
   doColorShift = false;
   
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";
	stateName[2]                    = "Fire";
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
};
datablock ItemData(SniperAmmostaticItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system
	shapeFile = "./bl_ammoSniper.dts";
	mass = 2;
	density = 1.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Ammo, Big Rifle Rounds";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	ammocount = 5*2;
	image = SniperAmmostaticImage;
};
datablock ShapeBaseImageData(SniperAmmostaticImage)
{
   shapeFile = "./bl_ammoSniper.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" );
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = SniperAmmostaticItem;
   ammo = " ";
   casing = gunShellDebris;
   shellExitDir        = "1.0 -1.3 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 15.0;	
   shellVelocity       = 7.0;
   melee = false;
   armReady = true;
   doColorShift = false;
   
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";
	stateName[2]                    = "Fire";
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
};
datablock ItemData(ShotgunAmmostaticItem)
{
	category = "Weapon";
	className = "Weapon";
	shapeFile = "./bl_ammoShotgun.dts";
	mass = 2;
	density = 1.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Ammo, Shotgun Rounds";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	ammocount = 5*3;
	image = ShotgunAmmostaticImage;
};
datablock ShapeBaseImageData(ShotgunAmmostaticImage)
{
   shapeFile = "./bl_ammoShotgun.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" );
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = ShotgunAmmostaticItem;
   ammo = " ";
   casing = gunShellDebris;
   shellExitDir        = "1.0 -1.3 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 15.0;	
   shellVelocity       = 7.0;
   melee = false;
   armReady = true;
   doColorShift = false;
   
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";
	stateName[2]                    = "Fire";
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
};
datablock ItemData(ExploAmmostaticItem)
{
	category = "Weapon";
	className = "Weapon";
	shapeFile = "base/data/shapes/brickweapon.dts";
	mass = 2;
	density = 1.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Ammo, Explosives";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	canDrop = true;
	ammocount = 2;
	image = ExploAmmostaticImage;
};
datablock ShapeBaseImageData(ExploAmmostaticImage)
{
   shapeFile = "base/data/shapes/brickweapon.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" );
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = ExploAmmostaticItem;
   ammo = " ";
   casing = gunShellDebris;
   shellExitDir        = "1.0 -1.3 1.0";
   shellExitOffset     = "0 0 0";
   shellExitVariance   = 15.0;	
   shellVelocity       = 7.0;
   melee = false;
   armReady = true;
   doColorShift = false;
   
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";
	stateName[2]                    = "Fire";
	stateFire[2]                    = true;
	stateAllowImageChange[2]        = false;
	stateSequence[2]                = "Fire";
	stateScript[2]                  = "onFire";
	stateWaitForTimeout[2]			= true;
};

function checkArmor(%client)
{
	if(%client.player.armored)
	{
		%p = %client.player;
		%p.unmountimage( 3 );
		%p.mountimage( armorKevlarEquippedImage, 3 );
	}
}

//Correctly set the environment of the server
package setupEnv
{
	function ServerLoadSaveFile_End()
	{
		parent::ServerLoadSaveFile_End();
		echo("Done loading bricks, applying environment.");
		setSkyBox("Add-Ons/Sky_DynamicWR/dynamic.dml");
		loadDayCycle("Add-ons/DayCycle_Teneksi/siten1.daycycle");
		dayCycle.setDayLength(7200);
	}
};
activatePackage(setupEnv);

//-----------------------------------------------------
//Vehicle Engine sounds (also includes far audio class)
//-----------------------------------------------------
exec("./engineSound.cs");

//-----------------------------------------------------
//Randomized Player
//-----------------------------------------------------
exec("./randomPlayer.cs");

//-----------------------------------------------------
//Floating Bricks support
//-----------------------------------------------------
exec("./floating.cs");

//-----------------------------------------------------
//Brick Materials
//-----------------------------------------------------
exec("./brickMats.cs");

//-----------------------------------------------------
//Passout stuff when damaged
//-----------------------------------------------------
//exec("./passout.cs");

//-----------------------------------------------------
//Sleeping
//-----------------------------------------------------
exec("./sleeping.cs");

//-----------------------------------------------------
//Item spawning
//-----------------------------------------------------
exec("./itemSpawning.cs");

//-----------------------------------------------------
//Zombies
//-----------------------------------------------------
exec("./zombies.cs");

//-----------------------------------------------------
//Moodles
//-----------------------------------------------------
exec("./moodles.cs");

//-----------------------------------------------------
//Local chat
//-----------------------------------------------------
exec("./localChat.cs");

//-----------------------------------------------------
//Weather System
//-----------------------------------------------------
//exec("./weather.cs");

//-----------------------------------------------------
//"DREAM WORLD"
//-----------------------------------------------------


//-----------------------------------------------------
//Weapon sound layering
//-----------------------------------------------------
exec("./weaponLayer.cs");

//-----------------------------------------------------
//Vehicle Impact modded
//-----------------------------------------------------
exec("./impact.cs");

//-----------------------------------------------------
//Voice emotes condensed
//-----------------------------------------------------
exec("./emotes.cs");

//-----------------------------------------------------
//Medical syringes
//-----------------------------------------------------
exec("./syringes/server.cs");

//-----------------------------------------------------
//Fixed gas mod
//-----------------------------------------------------
exec("./gas.cs");

//-----------------------------------------------------
//Combat logging fix
//-----------------------------------------------------
exec("./combat.cs");

//-----------------------------------------------------
//Replacement for AIConnection
//-----------------------------------------------------
exec("./fakeclient.cs");