using System.Collections.Generic;

public class HealthSubject
{
    private List<HealthObserver> observers = new List<HealthObserver>();

    public void Attach(HealthObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(HealthObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(int health)
    {
        foreach (HealthObserver observer in observers)
        {
            observer.UpdateHealth(health);
        }
    }
}

public interface HealthObserver
{
    void UpdateHealth(int health);
}
