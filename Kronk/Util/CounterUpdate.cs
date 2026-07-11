namespace Kronk.Util
{
    public class CounterUpdate
    {
        private readonly int MAXOBJECTS;

        public CounterUpdate(int mAXOBJECTS)
        {
            MAXOBJECTS = mAXOBJECTS;
        }

        internal void CounterUpdated(int currentNum)
        {
            Display.UpdateText();

            PlayerData.instance.ghostCoins = currentNum;

            if (currentNum == MAXOBJECTS)
            {
                Kronk.SendMessageToLivesplit();
            }
        }
    }
}
