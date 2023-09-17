using UnityEngine;

public class TreeController : MonoBehaviour, IAttackable
{
    public int treeMaxHealth = 50;
    private int currentHealth;

    private void Start()
    {
        currentHealth = treeMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            CutDown();
        }
    }

    private void CutDown()
    {
        Destroy(gameObject);
    }
}
