using Godot;

public class Global {
    private static Global _instance;

    public static Global Instance { get {
        if (_instance == null) {
            _instance = new Global();
        }
        return _instance;
    } private set {}}
    
    public PlayerScore GameScore;
    public PhysicsCar Car;

    public void Reset() {
        GameScore = null;
        Car = null;
    }
}