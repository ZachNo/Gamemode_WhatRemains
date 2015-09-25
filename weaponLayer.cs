//by ZSNO
//Plays "fake" sound for gunshots that are too far away to hear

//Weapon long sound
datablock AudioProfile(FarSniperFireSound)
{
   filename    = "./sounds/farSniperFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarAssaultFireSound)
{
   filename    = "./sounds/farAssaultFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarLMGFireSound)
{
   filename    = "./sounds/farLMGFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarAssaultFire2Sound)
{
   filename    = "./sounds/farAssaultFire2.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarSniperFire2Sound)
{
   filename    = "./sounds/farSniperFire2.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarPistolFireSound)
{
   filename    = "./sounds/farPistolFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarShotgunFireSound)
{
   filename    = "./sounds/farShotgunFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarSMGFireSound)
{
   filename    = "./sounds/farSMGFire.wav";
   description = audioFar3d;
   preload = true;
};
datablock AudioProfile(FarExplodeFireSound)
{
   filename    = "./sounds/farExplosionFire.wav";
   description = audioFar3d;
   preload = true;
};

function playRangedSound(%obj, %sound, %min, %max)
{
	%count = ClientGroup.getCount();
	for(%i=0;%i<%count;%i++)
	{
		%client = ClientGroup.getObject(%i);
		if(!isObject(%client.player) || !isObject(%obj))
			return;
		%dist = vectorDist(%obj.getPosition(),%client.player.getPosition());
		
		//if(%dist > %min)
		//	echo(%dist);
		if(%dist > %min && %dist < %max)
		{
			%vectorDir = vectorNormalize(vectorSub(%client.player.getPosition(),%obj.getPosition()));
			%soundDist = 10 + %dist/20;
			%vectorToPlayer = vectorAdd(%client.player.getPosition(), vectorScale(%vectorDir,%soundDist));
			%client.play3D(%sound, %vectorToPlayer);
		}
	}
}
package longRangeWeapon
{
	//Explosions!!!!!!!!!
	function projectileData::onExplode(%this, %obj, %pos)
	{
		%ex = %this.explosion;
		if(%ex.impulseForce >= 1000 || %ex.radiusDamage >= 100)
			schedule(500,0,playRangedSound,%obj,"FarExplodeFireSound",10,5000);
		parent::onExplode(%this, %obj, %pos);
	}

	//BKT_ATAC
	function AIAWMFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSniperFire2Sound",100,3000);
		parent::AIAWMFire(%this,%obj,%slot,%val);
	}
	function G18FAOFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarPistolFireSound",100,1800);
		parent::G18FAOFire(%this,%obj,%slot,%val);
	}
	function HKL86Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarLMGFireSound",100,2400);
		parent::HKL86Fire(%this,%obj,%slot,%val);
	}
	function M4ComFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFireSound",100,2200);
		parent::M4ComFire(%this,%obj,%slot,%val);
	}
	function CM1911A1Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarPistolFireSound",100,1800);
		parent::CM1911A1Fire(%this,%obj,%slot,%val);
	}
	function NAVSEA14Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFire2Sound",100,2200);
		parent::NAVSEA14Fire(%this,%obj,%slot,%val);
	}
	
	//BKT_Combat
	function DARFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFire2Sound",100,2200);
		parent::DARFire(%this,%obj,%slot,%val);
	}
	function JNG90Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSniperFireSound",100,3000);
		parent::JNG90Fire(%this,%obj,%slot,%val);
	}
	function Moss500Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarShotgunFireSound",100,2200);
		parent::Moss500Fire(%this,%obj,%slot,%val);
	}
	function HKMP5KFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSMGFireSound",100,1800);
		parent::HKMP5KFire(%this,%obj,%slot,%val);
	}
	
	//BKT_Enforce
	function MPDRFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSMGFireSound",100,1800);
		parent::MPDRFire(%this,%obj,%slot,%val);
	}
	function FNP90USGFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSMGFireSound",100,1800);
		parent::FNP90USGFire(%this,%obj,%slot,%val);
	}
	function FSPAS12Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarShotgunFireSound",100,2200);
		parent::FSPAS12Fire(%this,%obj,%slot,%val);
	}
	function HKUSP45Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarPistolFireSound",100,1800);
		parent::HKUSP45Fire(%this,%obj,%slot,%val);
	}
	
	//BKT_Rebel
	function SUAK47Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFireSound",100,2200);
		parent::SUAK47Fire(%this,%obj,%slot,%val);
	}
	function SUAKMFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFire2Sound",100,2200);
		parent::SUAKMFire(%this,%obj,%slot,%val);
	}
	function FNFALDMRFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFire2Sound",100,2200);
		parent::FNFALDMRFire(%this,%obj,%slot,%val);
	}
	function HK21Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarAssaultFireSound",100,2200);
		parent::HK21Fire(%this,%obj,%slot,%val);
	}
	function MMSFire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarPistolFireSound",100,1800);
		parent::MMSFire(%this,%obj,%slot,%val);
	}
	function PP90M1Fire(%this,%obj,%slot,%val)
	{
		schedule(500,0,playRangedSound,%obj,"FarSMGFireSound",100,1800);
		parent::PP90M1Fire(%this,%obj,%slot,%val);
	}
};
activatePackage(longRangeWeapon);