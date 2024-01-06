namespace BetterRocketDesigns.utils
{
    class CraftTools
    {
        public static ConfigNode TransformAsConfigNode(Part rootPart)
        {
            ShipConstruct shipConstruct = new ShipConstruct();

            AddPartsToShipConstruct(rootPart, shipConstruct);

            return shipConstruct.SaveShip();
        }

        private static void AddPartsToShipConstruct(Part part, ShipConstruct shipConstruct)
        {
            shipConstruct.Add(part);

            foreach (Part childPart in part.children)
            {
                AddPartsToShipConstruct(childPart, shipConstruct);
            }
        }
    }
}
