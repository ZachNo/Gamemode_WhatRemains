//By ZSNO
//Currently does not work

//-----------------------------------------------------
//Passout stuff when damaged
//-----------------------------------------------------
$Survive::passOutDamage = 30;
$Survive::passOutChance = 30;
$Survive::unPassoutChance = 5;
$Survive::passoutCheckTime = 60000;
package SurvivePass
{
	function Armor::damage(%this, %obj, %src, %pos, %damage, %type)
	{
		parent::damage(%this, %obj, %src, %pos, %damage, %type);
		if(%damage > $survive::passoutdamage && isObject(%obj.client) && isObject(%obj))
		{
			if(getRandom(1,100) < $survive::passoutchance)
				%obj.client.passout();
		}
	}
};
activatePackage(SurvivePass);

function GameConnection::passOut(%this)
{
	if(isObject(%this.player))
	{
		%this.player.ispassedOut = 2;
		%this.player.passedOutTicks = 4;
		%this.player.enterSleep();
	}
}