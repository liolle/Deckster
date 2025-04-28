using deckster.utils;

namespace deckster.entities; 

public class CardEntity(string id, string name, int cost, int defense, int strength, string image)
{
  public string Id {get;init;} = id;
  public string Name  {get;init;} = name;
  public int Defense {get;init;} = defense;
  public int Cost {get;init;} = cost;
  public int strength {get;init;} = strength;
  public string Image {get;init;} = image;

  public static CardEntity Create( string name, int cost, int defense, int strenght, string image){
    return  new(IdGererator.GenerateId("CRD"), name, cost, defense, strenght, image);
  }
}

