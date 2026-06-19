using System;

namespace DuckGame.UFFMod
{
    internal class UffFunctionGive : UffFunction
    {
        private Type itemType;

        public UffFunctionGive(Type _itemType)
        {
            itemType = _itemType;
        }

        public override void Call(Duck duck)
        {
            if (duck.isServerForObject)
            {
                Holdable item = Activator.CreateInstance(itemType, Editor.GetConstructorParameters(itemType)) as Holdable;
                Level.Add(item);
                if (duck.ragdoll != null)
                    item.position = duck.ragdoll.position;
                else
                    item.position = duck.position;
                duck.GiveHoldable(item);
            }
        }
    }
}
