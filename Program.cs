using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory_Activity_36
{
    internal class Program
    {
        public enum Unit
        {
            Grams,
            Milliliters,
            Teaspoons,
            Tablespoons,
            Cups,
            Pieces
        }

        public class Ingredient
        {
            public string Name { get; set; }
            public decimal Quantity { get; set; }
            public Unit Unit { get; set; }
            public decimal BaseServings { get; set; }

            public Ingredient(string name, decimal quantity, Unit unit, decimal baseServings)
            {
                Name = name;
                Quantity = quantity;
                Unit = unit;
                BaseServings = baseServings;
            }

            public decimal GetScaledQuantity(decimal targetServings)
            {
                return Quantity * (targetServings / BaseServings);
            }

            public decimal GetNormalizedQuantity(Unit targetUnit)
            {
                switch (Unit)
                {
                    case Unit.Grams:
                        return Quantity;
                    case Unit.Milliliters:
                        return Quantity;
                    case Unit.Teaspoons:
                        return Quantity * 5m;
                    case Unit.Tablespoons:
                        return Quantity * 15m;
                    case Unit.Cups:
                        return Quantity * 240m;
                    case Unit.Pieces:
                        return Quantity;
                    default:
                        return Quantity;
                }
            }
        }

        public class PantryItem
        {
            public string Name { get; set; }
            public decimal Quantity { get; set; }
            public Unit Unit { get; set; }

            public PantryItem(string name, decimal quantity, Unit unit)
            {
                Name = name;
                Quantity = quantity;
                Unit = unit;
            }

            internal decimal GetNormalizedQuantity(Unit grams)
            {
                throw new NotImplementedException();
            }
        }

        public class RecipeProcessor
        {
            private List<Ingredient> recipeIngredients;
            private List<PantryItem> pantryItems;
            private decimal targetServings;

            public RecipeProcessor(List<Ingredient> ingredients, List<PantryItem> pantry, decimal targetServings)
            {
                recipeIngredients = ingredients;
                pantryItems = pantry;
                this.targetServings = targetServings;
            }

            public List<(string Name, decimal NeededQuantity, Unit Unit, decimal MissingQuantity)> CheckShortages()
            {
                var shortages = new List<(string, decimal, Unit, decimal)>();

                foreach (var ingredient in recipeIngredients)
                {
                    var scaledQuantity = ingredient.GetScaledQuantity(targetServings);
                    var pantryItem = pantryItems.FirstOrDefault(p =>
                        string.Equals(p.Name, ingredient.Name, StringComparison.OrdinalIgnoreCase));

                    if (pantryItem != null)
                    {
                        var pantryGrams = pantryItem.GetNormalizedQuantity(Unit.Grams);
                        var neededGrams = ingredient.GetNormalizedQuantity(Unit.Grams);

                        if (neededGrams > pantryGrams)
                        {
                            shortages.Add((
                                ingredient.Name,
                                scaledQuantity,
                                ingredient.Unit,
                                scaledQuantity - ConvertToOriginalUnit(neededGrams - pantryGrams, ingredient.Unit)
                            ));
                        }
                    }
                    else
                    {
                        shortages.Add((ingredient.Name, scaledQuantity, ingredient.Unit, scaledQuantity));
                    }
                }

                return shortages;
            }

            private decimal ConvertToOriginalUnit(decimal grams, Unit targetUnit)
            {
                switch (targetUnit)
                {
                    case Unit.Grams: return grams;
                    case Unit.Milliliters: return grams;
                    case Unit.Teaspoons: return grams / 5m;
                    case Unit.Tablespoons: return grams / 15m;
                    case Unit.Cups: return grams / 240m;
                    case Unit.Pieces: return grams;
                    default: return grams;
                }
            }
            public void PrintShoppingList()
            {
                Console.WriteLine($"Shopping List for {targetServings} servings:");
                var shortages = CheckShortages();

                if (!shortages.Any())
                {
                    Console.WriteLine("You have all ingredients needed!");
                    return;
                }

                foreach (var (name, needed, unit, missing) in shortages)
                {
                    Console.WriteLine($"{name}: Need {Math.Round(needed, 2)} {unit}, Missing {Math.Round(missing, 2)} {unit}");
                }
            }
        }

        public class Prog
        {
            public static void Main(string[] args)
            {
                var ingredients = new List<Ingredient>
        {
            new Ingredient("Flour", 200, Unit.Grams, 2),
            new Ingredient("Sugar", 50, Unit.Grams, 2),
            new Ingredient("Milk", 300, Unit.Milliliters, 2),
            new Ingredient("Eggs", 2, Unit.Pieces, 2),
            new Ingredient("Vanilla Extract", 2, Unit.Teaspoons, 2)
        };

                var pantry = new List<PantryItem>
        {
            new PantryItem("Flour", 300, Unit.Grams),
            new PantryItem("Sugar", 30, Unit.Grams),
            new PantryItem("Milk", 500, Unit.Milliliters),
            new PantryItem("Eggs", 1, Unit.Pieces)

        };

                Console.WriteLine("Enter target number of servings:");
                decimal targetServings = decimal.Parse(Console.ReadLine());

                var processor = new RecipeProcessor(ingredients, pantry, targetServings);
                processor.PrintShoppingList();
            }
        }
    }
}