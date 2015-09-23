	datablock ItemData(fpCheeseburgerItem)
	{
		category = "Weapon";  // Mission editor category
		className = "Weapon"; // For inventory system

		 // Basic Item Properties
		shapeFile = "./fpCheeseburger.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;

		//gui stuff
		uiName = "Cheeseburger";
		iconName = "./Icon_fpCheeseburger";
		doColorShift = false;

		 // Dynamic properties defined by the scripts
		image = fpCheeseburgerImage;
		canDrop = true;
	};

	datablock ShapeBaseImageData(fpCheeseburgerImage)
	{
	   // Basic Item properties
	   shapeFile = "./fpCheeseburger.dts";
	   emap = true;

	   // Specify mount point & offset for 3rd person, and eye offset
	   // for first person rendering.
	   mountPoint = 0;
	   offset = "-.05 0 0";
	   eyeOffset = 0; //"0.7 1.2 -0.5";
	   rotation = eulerToMatrix( "-90 90 0" );

	   className = "WeaponImage";
	   item = fpCheeseburgerItem;

	   //raise your arm up or not
	   armReady = true;

	   doColorShift = false;

	   // Initial start up state
		stateName[0]                     = "Ready";
		stateTransitionOnTriggerDown[0]  = "Fire";
		stateAllowImageChange[0]         = true;

		stateName[1]                     = "Fire";
		stateTransitionOnTimeout[1]      = "Ready";
		stateAllowImageChange[1]         = true;
		  stateScript[1]                   = "onFire";
		stateTimeoutValue[1]		   = 1;
		stateSound[1]					= EatSound;
	};

	function fpCheeseburgerImage::onFire(%this,%obj,%slot)
	{
		

		for(%i=0;%i<5;%i++)
		{
			%toolDB = %obj.tool[%i];
			if(%toolDB $= %this.item.getID())
			{
				%obj.addhunger(30);
				%obj.tool[%i] = 0;
				%obj.weaponCount--;
				messageClient(%obj.client,'MsgItemPickup','',%i,0);
				serverCmdUnUseTool(%obj.client);
				break;
			}
		}
	}