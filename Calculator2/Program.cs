using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Calculator2
{
    /* *********************************************************************
     * Programmet bygger på en while loop som frågar efter inmatning och gör uträkningar till
     * dess att användaren väljer att avsluta. Jag har skapat en klass som genom sina egenskaper representerar 
     * en uträkning. Jag sparar sedan varje uträkning i en lista så att historik kan visas.
     * Jag har också skapat en statisk klass med metoder för uträkningarna och en klass som hanterar inmatning
     * och utmatning. Jag har försökt utfgå från DRY-principen så gott jag kunnat för att undvika 
     * att skriva samma kod flera gånger. Det gör det enklare om något behöver ändras och därför har jag en del
     * fristående metoder.
     * 
     * Allt ligger i den här filen, även de separata klasserna. Jag är medveten om att varje klass egentligen bör finnas
     * i en egen fil men jag har allt i samma den här gången för att det ska vara enkelt att lämna in uppgiften.
     * ********************************************************************* */

    class Program
    {
        static void Main(string[] args)
        {
            /* *************************************************************************
             * Jag sparar alla uträkningar i en lista med objekt som sedan kan användas för att
             * visa historiken. Eftersom antalet uträkningar beror på användaren och inte är bestämt på förhand
             * har jag valt att använda en lista. Jag har funderat på andra alternativ för att spara uträkningarna, 
             * exempelvis array och förstora men en lista lämpar sig väl för ändamålet.             
             * ************************************************************************* */            
            List<MathOperation> operations = new List<MathOperation>();
            string input;
            
            bool firstIteration = true;
            // Den här loopen kommer att köras så länge användaren inte väljer att avsluta programmet.
            // Logik för att avsluta finns inuti loopen.
            while(true)
            {
                Console.Clear();
                // Om det är första iterationen av while-loopen visas instruktioner
                if (firstIteration)
                {                    
                    HandleIO.DisplayInstructions();                   
                    firstIteration = false;
                }

                /* *
                 * Om det finns tidigare uträkningar ska dessa visas                  
                 * Metodens andra agrument anger hur mycket av historiken som ska visas.
                 * För hela uträkningar = all
                 * För endast summeringar = results
                 * */
                if (operations.Count > 0) HandleIO.DisplayOperations(operations, view: "all");

                /*
                 * Varje uträkning sparas som en instans av klassen MathOperation vilken representerar
                 * en uträknings alla delar. 
                 * 
                 * Ett alternativ till den här lösningen skulle kunna vara att spara alla delar av uträkningen i separata 
                 * variabler som definieras i main-metoden men jag har valt att utgå från ett objekt eftersom jag tycker 
                 * det är smidigt att ha alla delar av uträkningen samlade på ett ställe. 
                 * Dessutom kommer jag att lagra informationen i ett objekt i en lista senare.
                 */
                MathOperation operation = new MathOperation();

                // Visa prompt och inmatning för första termen eller motsvarande
                // Lagra den första termen i objektet för uträkningen
                Console.Write("#: ");
                input = Console.ReadLine().Trim();
                operation.Number1 = HandleIO.GetNumber(input);

                // Visa prompt och inmatning för den matematiska operatorn
                // Lagra operatorn i objektet för uträkningen
                Console.Write(": ");
                input = Console.ReadLine().Trim();
                operation.MathOperator = HandleIO.GetOperator(input);

                // Visa prompt och inmatning för den andra termen
                // Lagra den andra termen i objektet för uträkningen
                Console.Write("#: ");
                input = Console.ReadLine().Trim();
                operation.Number2 = HandleIO.GetNumber(input);

                // Räkna ut och lagra resultatet
                operation.Result = Calculator.DoOperation(operation.Number1, operation.Number2, operation.MathOperator);                
                
                // Lagra uträkningen i listan med uträkningar
                operations.Add(operation);
            }                      
        }      
    }

    /******************************************************************
     * En statisk klass med metoder för att hantera in- och utmatning
     *****************************************************************/    
    public static class HandleIO
    {
        // Metoden kontrollerar att användaren skriver in ett nummer för att sedan returnera det
        public static double GetNumber(string input)
        {
            // Kontrollera om inmatningen innebär att användaren vill avsluta programmet
            CheckIfExitProgram(input);           
            
            /* För att minska risken att användaren knappar in för stora tal är maxlängden på inmatningen
            * satt till 14 tecken. Med datatypen double är precisionen kanske inte tillräcklig för en riktig miniräknare
            * men jag bedömer att det får räcka för den här applikationen. Datatypen double ska kunna hantera ~15-17 siffror.
            *
            * Det här fungerar för de flesta "vanliga" uträkningarna men talen kan ändå bli för stora om användaren exempelvis
            * multiplicerar med stora tal. Jag har tittat lite på Microsofts OverflowException-klass men jag har inte hunnit
            * implementera något som fungerar fullt ut för att helt eliminera risken.
            */
            const int MaxLength = 14;
            bool tooLong = false;
            double number;

            // Kontrollera om längden på inmatning är längre än antalet tillåtna tecken
            if (input.Length > MaxLength) tooLong = true;

            // Försök att omvandla textsträngen till ett nummer. 
            // Be annars användaren att fylla i ett nummer
            bool validNumber = double.TryParse(input, out number);            
            while (!validNumber || tooLong)
            {                
                Console.ForegroundColor = ConsoleColor.Red;
                if (!validNumber)
                {
                    Console.WriteLine("Du måste ange en siffra.");                    
                }
                else
                {
                    Console.WriteLine("Ange ett mindre tal");
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("#: ");
                input = Console.ReadLine().Trim();
                // Kontrollera återigen om användaren vill avsluta programmet
                CheckIfExitProgram(input);
                // Försök omvandla inmatningen till ett nummer
                validNumber = double.TryParse(input, out number);
                // Kontrollera återigen om inmatningen är för lång.                
                tooLong = (input.Length > MaxLength);
            }
            return number;
        }

        /* 
        * Metoden kontrollerar att användaren skriver in en giltig matematisk operator och returnerar den
        * Jag har valt att utgå från datatypen string även om operatorerna skulle kunna lagras som char
        * Med string är det enklare att hantera inmatning/utmatning då jag slipper konvertera string till
        * char i vissa lägen. Char är antagligen effektivare men string borde fungera lika bra 
        * i en så här simpel applikation.
        */
        public static string GetOperator(string input)
        {
            // Kontrollera om inputen innebär att användaren vill avsluta programmet
            CheckIfExitProgram(input, exitByName: true);
            // Giltiga operatorer
            string[] validOperators = { "+", "-", "/", "*" };            
            // Kontrollera om inmatningen från användaren stämmer med en giltig operator
            while (!validOperators.Contains(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Du måste ange en giltig operator + - / *");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(": ");
                input = Console.ReadLine().Trim();
                // Kontrollera om användaren vill avsluta programmet
                CheckIfExitProgram(input, exitByName: true);
            }
            return input;
        }

        // Metod för att kontrollera om användaren vill avsluta programmet
        // Den första paremetern är inmatningen från användaren
        // Den andra parametern avgör om det går att avsluta programmet genom att mata in ett namn i stället
        private static void CheckIfExitProgram(string input, bool exitByName = false)
        {
            // Giltiga parametrar för att avsluta programmet
            string[] exitParams = { "q", "quit" };
            // Det ska gå att avsluta genom att byta ut operatorn mot MARCUS
            const string ExitName = "MARCUS";

            if (exitParams.Contains(input))
            {
                Console.WriteLine();
                Console.WriteLine("Programmet avslutades");
                Thread.Sleep(2000);
                Environment.Exit(0);
            // Om det går att avsluta genom att skriva ett namn kontrolleras om kriterierna för det är uppfyllda
            } else if(input == ExitName && exitByName)
            {
                Console.WriteLine();
                Console.WriteLine("Hej!");
                Thread.Sleep(2000);
                Environment.Exit(0);
            }
        }
        // Metoden skriver ut instruktioner
        public static void DisplayInstructions()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("########## MINIRÄKNARE ##########\n");
            Console.WriteLine(
                "INSTRUKTIONER:\n" +
                " 1. Mata in ett tal,\n" +
                " 2. Mata in en operator (+, -, / eller *)\n" +
                " 3. Mata in ytterligare ett tal\n" +
                " - Tryck enter efter varje val\n" +
                " - Skriv q eller quit för att avsluta\n");
            Console.WriteLine("-----------------------------------------------------\n");            
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Metoden skriver ut tidigare uträkningar
        public static void DisplayOperations(List<MathOperation> operations, string view)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Tidigare uträkningar: ");
            // Här går det att använda en foreach-loop också (eller något annat så som while eller do/while)
            // Generellt tycker jag att läsbarheten är högre med foreach. Eftersom
            // vi pratat mycket om for-loopar i kursen har jag ändå utgått från en sådan här.
            for (int i = 0; i < operations.Count; i++)
            {
                if (operations[i].MathOperator == "/" && operations[i].Number2 == 0)
                {
                    // Om användaren försökt dela med noll har ingen uträkning utförts.
                    // Då visas ett felmeddelande...
                    Console.WriteLine(" Det går inte att dela med noll!");
                }
                else
                {
                    // ... annars visas uträkningen
                    // Parametern view avgör hur mycket information som ska visas                 
                    if(view =="all") Console.WriteLine($" {operations[i].Number1} {operations[i].MathOperator} {operations[i].Number2} = {operations[i].Result}");
                    if(view =="results") Console.WriteLine($" {operations[i].Result}");
                }

            }
            Console.WriteLine("-----------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }        
    }

    /******************************************************************
    * En klass som innehåller egenskaper motsvarande en uträkning
    * 
    * Jag har valt att skapa en klass för att lagra alla delar av en uträkning
    * Jag har sneglat lite på struct, tuples och dictionary men tycker att en klass löser
    * problemet på ett enkelt sätt eftersom det håller ihop de delar som hör samman och 
    * gör det möjligt att lagra de olika datatyperna double/string i ett och samma objekt
    * på ett smidigt sätt. 
    * 
    * Utöver de olika termerna och operatorn har jag även valt att
    * lagra resultatet. Det räknas ut i samband med att termer och operator matas in
    * men skulle också kunna räknas ut och visas i samband med att jag skriver ut historiken. 
    * Jag har dock valt att spara alla delar så här vilket gör det möjligt att, när som helst, 
    * få tillgång till resultatet av en uträkning utan att behöva utföra den flera gånger.
    *****************************************************************/    
    public class MathOperation
    {        
        public double Number1 { get; set; }
        public double Number2 { get; set; }
        public string MathOperator { get; set; }
        public double Result { get; set; }
    }

    /******************************************************************
    * En statisk klass med metoder som genomför uträkningar
    * Jag har valt att skapa en separat klass för att utföra uträkningarna. Dessa metoder skulle
    * kunna ingå i klassen MathOperation men jag har valt att lägga dem separat.
    * Jag hanterar bara en uträkning i taget och därför passar det bra att ha dessa metoder statiska. 
    * Att hålla metoderna i en separat klass gör också att de kan återanvändas lättare i andra sammanhang. 
    * Funktionaliteten är inte bunden till de specifika egenskaperna som hör till en uträkning i just det här programmet.
    *******************************************************************/
    public static class Calculator
    {
        /******************************************************************
        * Metoden hanterar alla uträkningar och anropar rätt metod baserat på angiven operator
        * På så sätt räcker det med att man anropar en metod för att utföra en uträkning,
        * så långe operatorn är korrekt kommer sedan klassen att hantera
        * vilken typ av uträkning som ska genomföras. Metoderna är privata vilket innebär att 
        * den som ska använda klassen inte kan/behöver anropa mer än en metod.
        *******************************************************************/

        public static double DoOperation(double number1, double number2, string op)
        {            
            double result = 0;
            switch(op)
            {
                case "+":
                    result = Add(number1, number2);
                    break;
                case "-":
                    result = Subtract(number1, number2);
                    break;
                case "/":
                    if(number2 != 0)
                    {
                        result = Divide(number1, number2);
                    }
                    break;
                case "*":
                    result = Multiply(number1, number2);
                    break;
            }
            return Math.Round(result, 10);
        }
        // En metod för respektive uträkning
        private static double Add(double number1, double number2)
        {
            return number1 + number2;
        }
        private static double Subtract(double number1, double number2)
        {
            return number1 - number2;
        }
        private static double Divide(double number1, double number2)
        {
            return number1 / number2;
        }
        private static double Multiply(double number1, double number2)
        {
            return number1 * number2;
        }
    }    
}
