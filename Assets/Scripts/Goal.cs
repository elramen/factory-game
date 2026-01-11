public class Goal {
    public int level;
    public Item item;
    public int amount;
    public CoolColor[,] pixels;
    // Could also be implemented in a later stage 
    // int xpReward
    // String text

    public Goal(int level, Item item, int amount) {
        this.level = level;
        this.item = item;
        this.amount = amount;
    }
}