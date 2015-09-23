//Support_Impact_Damage by BluetoothBoy
//Fixed by ZSNO
//Usage instructions:

//Add "playerDamage = X;" to vehicle datablock for the minimum amount of health taken away upon impact (impact damage takes place at a speed of 25 or greater). Default = 5. Player(s) will die upon impact with a speed of 75 or greater.
//Function for toggling damage.

$impactDamageToggled = true;

function servercmdimpactdamage(%c)
{
	if(%c.isAdmin || %c.isSuperAdmin || %c.isHost)
	{
		if($impactDamageToggled)
		{
			%c.chatmessage("\c3Impact damage \c0off!");
			$impactDamageToggled = false;
		}
		else
		{
			%c.chatmessage("\c3Impact damage \c2on!");
			$impactDamageToggled = true;
		}
	}
	else
		%c.chatmessage("\c3You need to be admin to toggle impact damage.");
}
function collided(%this, %obj, %col, %pos, %speed)
{
	for(%i = 0; %i < %obj.getDataBlock().numMountPoints; %i++)
	{
		if(%obj.getMountedObject(%i) != 0 && %obj.getMountedObject(%i).getDataBlock().rideable == false)
		{
			%currentPlayer = %obj.getMountedObject(%i);
			if(%speed >= 25 && %speed < 75)
			{
				if(%obj.getDatablock().playerDamage)
					%currentPlayer.applyDamage(%obj.getDataBlock().playerDamage*(%speed/25));
				else
					%currentPlayer.applyDamage(5*(%speed/25));
			}
			else if(%speed >= 75)
				%currentPlayer.kill();					
		}
	}
}

function WheeledVehicleData::onImpact(%this, %obj, %col, %pos, %speed)
{
	if($impactDamageToggled)
		collided(%this, %obj, %col, %pos, %speed);
}
function FlyingVehicleData::onImpact(%this, %obj, %col, %pos, %speed)
{
	if($impactDamageToggled)
		collided(%this, %obj, %col, %pos, %speed);
}