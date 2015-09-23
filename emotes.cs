//originally by someone else (can't find original author)
//modified to put all emotes in same file (originally all seperate)

//server emotes
datablock audioProfile( burpSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/burp.wav";
	preload = true;
};
datablock audioProfile( clapSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/clap.wav";
	preload = true;
};
datablock audioProfile( coughSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/cough.wav";
	preload = true;
};
datablock audioProfile( dumbassSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/dumbass.wav";
	preload = true;
};
datablock audioProfile( fartSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/fart.wav";
	preload = true;
};
datablock audioProfile( greetSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/greet.wav";
	preload = true;
};
datablock audioProfile( laughSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/laugh.wav";
	preload = true;
};
datablock audioProfile( noSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/no.wav";
	preload = true;
};
datablock audioProfile( notmadSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/notmad.wav";
	preload = true;
};
datablock audioProfile( pleaseSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/please.wav";
	preload = true;
};
datablock audioProfile( psstSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/psst.wav";
	preload = true;
};
datablock audioProfile( shhSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/shh.wav";
	preload = true;
};
datablock audioProfile( sighSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/sigh.wav";
	preload = true;
};
datablock audioProfile( thanksSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/thanks.wav";
	preload = true;
};
datablock audioProfile( woohooSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/woohoo.wav";
	preload = true;
};
datablock audioProfile( yellSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/yell.wav";
	preload = true;
};
datablock audioProfile( yesSound )
{
	description = "audioClosest3D";
	fileName = "./emotes/yes.wav";
	preload = true;
};

function serverCmdburp( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastburpSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( burpSound, %pl.getHackPosition() );
}
function serverCmdclap( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;

	if ( getSimTime() - %cl.lastclapSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( clapSound, %pl.getHackPosition() );
}
function serverCmdCough( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastcoughSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( coughSound, %pl.getHackPosition() );
}
function serverCmddumbass( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastdumbassSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( dumbassSound, %pl.getHackPosition() );
}
function serverCmdFart( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastFartSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( fartSound, %pl.getHackPosition() );
}
function serverCmdgreet( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastgreetSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( greetSound, %pl.getHackPosition() );
}
function serverCmdLaugh( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastLaughSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( laughSound, %pl.getHackPosition() );
}
function serverCmdno( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastnoSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( noSound, %pl.getHackPosition() );
}
function serverCmdnotmad( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastnotmadSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( notmadSound, %pl.getHackPosition() );
}
function serverCmdplease( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastpleaseSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( pleaseSound, %pl.getHackPosition() );
}
function serverCmdpsst( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastpsstSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( psstSound, %pl.getHackPosition() );
}
function serverCmdshh( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastshhSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( shhSound, %pl.getHackPosition() );
}
function serverCmdsigh( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastsighSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( sighSound, %pl.getHackPosition() );
}
function serverCmdthanks( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastthanksSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( thanksSound, %pl.getHackPosition() );
}
function serverCmdwoohoo( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastwoohooSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( woohooSound, %pl.getHackPosition() );
}
function serverCmdyell( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastyellSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( yellSound, %pl.getHackPosition() );
}
function serverCmdyes( %cl )
{
	if ( !isObject( %pl = %cl.player ) )
		return;
	if ( getSimTime() - %cl.lastyesSound < 1000 )
		return;
	%cl.lastFartSound = getSimTime();
	serverPlay3D( yesSound, %pl.getHackPosition() );
}