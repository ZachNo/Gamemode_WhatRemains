//Originally by Rotondo
//Modified by ZSNO

//-----------------------------------------------------
//Randomized Player
//-----------------------------------------------------
package randomizedPlayer
{
	function Gameconnection::spawnPlayer(%client)
	{
		parent::spawnPlayer(%client);
		if(isObject(%client.player) && !%client.randomed)
			%client.player.setRandomAppearance(0);
		%client.schedule(1000,applyBodyParts);
		%client.schedule(1000,applyBodyColors);
		%client.randomed = 1;
		
	}
	function Gameconnection::onDeath(%client)
	{
		%client.randomed = 0;
		parent::onDeath(%client);
	}
};
activatePackage(randomizedPlayer);

function Player::setRandomAppearance( %this, %style )
{
	%obj = %this.client;
	if(!isObject(%obj))
		return;

	%skinColor = getRandomBotSkinColor();
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
	%face = getRandomBotFace();
	if( %style == 0 )
	{
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
	}
	else if( %style == 1 )
	{
		%decal0 = "AAA-None";
		%decal1 = "Space-Nasa";
		%decal2 = "Space-New";
		%decal3 = "space-Old";
		%decal = %decal[ getRandom( 0, 3 ) ];
		%pack = 6;
		%hat = 1;
		%hatColor = getRandomBotOffsetColor( %shirtColor );
		%packColor = %hatColor;
		%handColor = %packColor;
		%pantsColor = getRandomBotOffsetColor( %shirtColor );
		%shoeColor = %pantsColor;
		%accent = 1;
		%accentColor = getRandomBotTransColor();
	}
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
	if(isObject(%obj))
	{
		%obj.ApplyBodyParts(%obj);
		%obj.ApplyBodyColors(%obj);
	}
}
RegisterPersistenceVar("accentColor",false,"");
RegisterPersistenceVar("accent",false,"");
RegisterPersistenceVar("hatColor",false,"");
RegisterPersistenceVar("hat",false,"");
RegisterPersistenceVar("headColor",false,"");
RegisterPersistenceVar("faceName",false,"");
RegisterPersistenceVar("chest",false,"");
RegisterPersistenceVar("decalName",false,"");
RegisterPersistenceVar("chestColor",false,"");
RegisterPersistenceVar("pack",false,"");
RegisterPersistenceVar("packColor",false,"");
RegisterPersistenceVar("secondPack",false,"");
RegisterPersistenceVar("secondPackColor",false,"");
RegisterPersistenceVar("larm",false,"");
RegisterPersistenceVar("larmColor",false,"");
RegisterPersistenceVar("lhand",false,"");
RegisterPersistenceVar("lhandColor",false,"");
RegisterPersistenceVar("rarm",false,"");
RegisterPersistenceVar("rarmColor",false,"");
RegisterPersistenceVar("rhand",false,"");
RegisterPersistenceVar("rhandColor",false,"");
RegisterPersistenceVar("hip",false,"");
RegisterPersistenceVar("hipColor",false,"");
RegisterPersistenceVar("llegColor",false,"");
RegisterPersistenceVar("rleg",false,"");
RegisterPersistenceVar("rlegColor",false,"");
RegisterPersistenceVar("randomed",false,"");