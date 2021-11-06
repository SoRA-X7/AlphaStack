namespace AlphaStack.Game {
    public abstract class Player {
        public int ID { get; set; }
        
        public abstract Field PlayerField { get; }

        public abstract bool Ready { get; }
        public abstract bool HasError { get; }

        public abstract bool SetRules(GameRules rules);

        public abstract void Prepare();

        public abstract void Start();

        public abstract void Stop();
    }
}