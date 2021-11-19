namespace Sources.Support {
    public class Singleton<T> where T : new() {
        private static T instance;

        protected Singleton() { }

        public static T getInstance() {
            if (instance == null)
                instance = new T();
            return instance;
        }
    }
}
