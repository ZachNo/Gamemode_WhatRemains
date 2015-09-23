//########## Syringes
$GCStuff::SyringeDuration = 60;

AddDamageType("gc_Syringe",'<bitmap:Add-ons/Item_Syringes/CI_syringe> %1','%2 <bitmap:Add-ons/Item_Syringes/CI_syringe> %1',0.2,1);

// exec("./baton.cs");
exec("./syringered.cs");
exec("./syringeyellow.cs");
exec("./syringecyan.cs");
exec("./syringegreen.cs");
exec("./syringeviolet.cs");

package gc_SyringesPackage
{
  function Armor::onTrigger(%this,%player,%slot,%val)
  {
    if(isObject(%player) && isObject(%player.getMountedImage(0))) {
      %image = %player.getMountedImage(0);
      if(%image.item.gc_syringe && %player.getImageState(0) $= "Ready" && %slot $= 4 && %val) %image.onSelfUse(%player,0);
    }
    parent::onTrigger(%this,%player,%slot,%val);
  }
};
activatePackage(gc_SyringesPackage);
