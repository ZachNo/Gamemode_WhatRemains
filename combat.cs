//By ZSNO

//-----------------------------------------------------
//Combat logging
//-----------------------------------------------------

$WR::CombatLogging = 60000;

new SimGroup(CombatLogged);

package WRCombatLogging
{
	function GameConnection::onDrop(%client, %reason)
	{
		for(%i = 0; %i < 5; %i++)
		{
			if(%player.toolammo[%a] > 0)
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
		}
		if(!isObject(%client.player) || %client.isAiControlled())
		{
			//echo("Player doesn't exist or delayed connection remove");
		}
		else
		{
			//echo("Delay client removal");
			
			%cf = copyClient(%client);
			
			%cf.player = copyPlayer(%client.player);
			%cf.player.client = %cf;
			%cf.applyBodyParts();
			%cf.applyBodyColors();
			
			%cf.player.playThread(0,sit);
			
			%cf.schedule($WR::CombatLogging - 1, savePersistence);
			%cf.schedule($WR::CombatLogging, delete);
			%cf.player.schedule($WR::CombatLogging, delete);
			
			CombatLogged.add(%cf);
		}
		Parent::onDrop(%client, %reason);
	}
	function GameConnection::onClientEnterGame(%client)
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%c = ClientGroup.getObject(%i);
			if(%c.bl_id == %client.bl_id && %c != %client)
			{
				%c.savePersistence();
				%c.onDrop("Other user in server using same account");
				//echo("Removed previous player");
			}
		}
		for(%i = 0; %i < CombatLogged.getCount(); %i++)
		{
			%c = CombatLogged.getObject(%i);
			//echo(%c.bl_id SPC %client.bl_id);
			if(%c.bl_id == %client.bl_id)
			{
				%c.savePersistence();
				%c.player.delete();
				%c.delete();
				//echo("Removed previous player");
			}
		}
		Parent::onClientEnterGame(%client);
	}
};
activatePackage(WRCombatLogging);

function FClient::savePersistence(%client)
{
      //client connected, but did not auth, seems like hasSpawnedOnce should catch this but it apparently doesn't
      if(%client.bl_id < 0 || %client.bl_id $= "")
         return;

      //open file
      %file = new FileObject();
      %filename = "config/server/persistence/" @ %client.bl_id @ ".txt";
      %file.openForWrite(%filename);

      if(!%file)
      {
         error("ERROR: AIConnection::savePersistence(" @ %client @ ") - failed to open file '" @ %filename @ "' for write");
         %file.delete();
         return;
      }

      echo("Saving persistence for BLID " @ %client.bl_id);


      //save all registered client tagged fields
      %file.writeLine(">CLIENT");
      Persistence::SaveTaggedFields(%client, %file);
      

      //save player object
      %player = %client.player;
      if(isObject(%player))
      {
         %file.writeLine(">PLAYER");
         %file.writeLine("datablock" TAB %player.getDataBlock().getName());
         %file.writeLine("transformPerMap" TAB $MapSaveName TAB %player.getTransform());
         %file.writeLine("velocity" TAB %player.getVelocity());
         %file.writeLine("damagePercent" TAB %player.getDamagePercent());
         %file.writeLine("scale" TAB %player.getScale());
         Persistence::SaveTaggedFields(%player, %file);
      }
	  else
	  {
		%file.writeLine(">CAMERA");
	  }

      //close file
      %file.close();
      %file.delete();
}

//function GameConnection::onDrop(%client, %reason)
//{
//	$Server::PlayerCount = ClientGroup.getCount();
//	if(!%client.connected)
//		return;
//	$Server::PlayerCount--;
//	%client.onClientLeaveGame();
//	removeFromServerGuidList(%client.guid);
//	if(!%client.isBanReject && %client.getHasAuthedOnce() && !$Server::LAN)
//	{
//		messageAllExpect(%client, '', %client.getPlayerName() SPC "has left the game");
//		secureCommandToAllExcept("zbR4HmJcSY8hdRhr", %client, 'ClientDrop', %client.getPlayerName(), %client);
//	}
//	echo("CDROP: " @ %client @ " " %client.getAddress());
//	if(!%client.isBanReject)
//	{
//		if($Server::PlayerCount == $Pref::Server::MaxPlayers - 1)
//		{
//			if(getSimTime() - $Server::lastPostTime > 30*1000 && !($Server::lastPostTime < 30*1000))
//				WebCom_PostServer();
//		}
//	}
//}
		