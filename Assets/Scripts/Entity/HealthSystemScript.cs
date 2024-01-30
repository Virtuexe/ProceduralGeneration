public class HealthSystemScript
{
    private float maxHealth;
    private float health;
    public HealthSystemScript(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
    }
    public void setHealth(float health)
    {
        this.health = health;
    }
    public float getHealth()
    {
        return health;
    }
    public void damage(float damage)
    {
        this.health -= damage;
        if(health > maxHealth) health = maxHealth;
    }
    public void heal(float damage)
    {
        this.health += damage;
        if (health > maxHealth) health = maxHealth;
    }
}
