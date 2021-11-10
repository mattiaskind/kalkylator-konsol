using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator2
{
    /* *
     * Programmet bygger på en while loop som frågar efter inmatning och gör uträkningar till
     * dess att användaren väljer att avsluta. Jag har skapat en klass som genom sina egenskaper representerar 
     * en uträkning. Jag sparar sedan varje uträkning i en lista så att historik kan visas.
     * Jag har också skapat en statisk klass med metoder för uträkningarna och en del separata metoder
     * för att undvika duplciering av kod.     
     * */
    class Program
    {
        static void Main(string[] args)
        {            
            /* 
             * Jag sparar alla uträkningar i en lista av objekt som sedan kan användas för att
             * visa historiken
             */
            List<MathOperation> operations = new List<MathOperation>();
            string input;
            
            bool firstIteration = true;
            while(true)
            {
                Console.Clear();
                // Om det är första iterationen av while-loopen visas instruktioner
                if (firstIteration)
                {                    
                    HandleIO.DisplayInstructions();                   
                    firstIteration = false;
                }

                // Om det finns tidigare uträkningar, visa dem
                // Andra argumentet anger hur mycket av historiken som ska visas.
                // För att visa hela historiken anges all, för endast summeringar: results
                if (operations.Count > 0) HandleIO.DisplayOperations(operations, "all");
                
                // Varje uträkning sparar som ett objekt
                // Initiera ett nytt objekt för uträkning
                MathOperation operation = new MathOperation();

                // Visa prompt och inmatning för första termen eller motsvarande
                // Lagra det första termen i objektet för uträkningen
                Console.Write("#: ");
                input = Console.ReadLine().Trim();
                operation.Number1 = HandleIO.GetNumber(input);

                // Visa prompt och inmatning för den matematiska operatorn
                // Lagra operatorn i objektet för uträkningen
                Console.Write(": ");
                input = Console.ReadLine().Trim();
                operation.MathOperator = HandleIO.GetOperator(input);

                // Visa prompt och inmatning för den andra termen
                // Lagra numret i objektet för uträkningen
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

    // En klass med statiska metoder för input och output
    public static class HandleIO
    {
        // Metoden kontrollerar att användaren skriver in ett nummer och sedan returnera det
        public static double GetNumber(string input)
        {
            // Kontrollera om inmatningen innebär att användaren vill avsluta programmet?
            CheckExitProgram(input);
            double number;
            // Försök att omvandla textsträngen till ett nummer.
            // Be annars användaren att fylla i ett nummer
            while (!double.TryParse(input, out number))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Du måste ange en siffra");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("#: ");
                input = Console.ReadLine().Trim();
                CheckExitProgram(input);
            }
            return number;
        }
        // Metoden kontrollerar att användaren skriver in en giltig matematisk operator och returnerar den
        public static string GetOperator(string input)
        {
            // Kontrollera om inputen innebär att användaren vill avsluta programmet?
            CheckExitProgram(input, true);
            // Giltiga operatorer
            string[] validOperators = { "+", "-", "/", "*" };            
            // Kontrollera om inmatningen från användaren stämmer med någon giltig operator
            while (!validOperators.Contains(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Du måste ange en giltig operator + - / *");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(": ");
                input = Console.ReadLine().Trim();
                CheckExitProgram(input, true);
            }
            return input;
        }

        // Metod för att kontrollera om användaren vill avsluta programmet
        // Den första paremetern är inmatningen från användaren
        // Den andra parametern avgör om det går att avsluta programmet genom att mata in ett namn i stället
        public static void CheckExitProgram(string input, bool exitByName = false)
        {
            // Giltiga parametrar för att avsluta programmet
            string[] exitParams = { "q", "quit" };

            if (exitParams.Contains(input))
            {
                Console.WriteLine();
                Console.WriteLine("Programmet avslutades");
                Environment.Exit(0);
            // Om det går att avsluta genom att skriva ett namn, kontrolleras om kriterierna för det är uppfyllda
            } else if(input == "MARCUS" && exitByName)
            {
                Console.WriteLine();
                Console.WriteLine("Hej!");
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
                " - Skriv ett tal, sedan +, -, / eller *\n" +
                " - Mata in ytterligare ett tal\n" +
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
            foreach (MathOperation listItem in operations)
            {   // Om användaren försökt dela med noll har ingen uträkning utförts.
                // Då visas ett felmeddelande, annars visas uträkningen
                if (listItem.MathOperator == "/" && listItem.Number2 == 0)
                {
                    Console.WriteLine("Det går inte att dela med noll!");
                }
                else
                {   // Parametern view avgör hur mycket information som ska visas                 
                    if(view =="all") Console.WriteLine($" {listItem.Number1} {listItem.MathOperator} {listItem.Number2} = {listItem.Result}");
                    if(view =="results") Console.WriteLine($"{listItem.Result}");
                }
            }Console.WriteLine("-----------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        
    }

    /* *************************************************************************
     * Jag har valt att skapa en klass för att lagra alla delar av en uträkning
     * Jag har sneglat lite på struct, tuples och dictionary men tycker att en klass löser
     * problemet på ett enkelt sätt eftersom en uträkning består av olika datatyper där
     * operatorn är en string/char
     * **************************************************************************/
    public class MathOperation
    {
        public double Number1 { get; set; }
        public double Number2 { get; set; }
        public string MathOperator { get; set; }
        public double Result { get; set; }
    }

    // En statisk klass med metoder som genomför uträkningar
    public static class Calculator
    {
        // Metoden hanterar alla uträkningar och anropar rätt metod baserat på angiven operator
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
            return result;
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
