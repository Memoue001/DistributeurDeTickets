using System;
using System.Collections.Generic;
using System.IO;

namespace DistributeurDeTickets
{
    internal class Client
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NumeroTicket { get; set; }

        public Client(string nom, string prenom, string numeroTicket)
        {
            Nom = nom;
            Prenom = prenom;
            NumeroTicket = numeroTicket;
        }

        public override string ToString()
        {
            return $"{Prenom} {Nom} - Ticket: {NumeroTicket}";
        }
    }

    internal class DistributeurDeTickets
    {
        private const string CheminFichierNumeros = @"C:\Users\DELL\OneDrive\Cours\S1M2\C#and .NET\numero.txt";
        private const string CheminFichierClients = @"C:\Users\DELL\OneDrive\Cours\S1M2\C#and .NET\clients.txt";
        private Dictionary<string, int> compteurTypes = new Dictionary<string, int>
        {
            { "V", 0 }, // Versement
            { "R", 0 }, // Retrait
            { "I", 0 }  // Informations
        };
        private List<Client> clients = new List<Client>();

        public DistributeurDeTickets()
        {
            ChargerNumeros();
            ChargerClients(); // Charger les clients depuis le fichier
        }

        private void ChargerNumeros()
        {
            if (File.Exists(CheminFichierNumeros))
            {
                string[] lignes = File.ReadAllLines(CheminFichierNumeros);
                foreach (string ligne in lignes)
                {
                    string[] parts = ligne.Split(':');
                    if (parts.Length == 2 && compteurTypes.ContainsKey(parts[0]))
                    {
                        compteurTypes[parts[0]] = int.Parse(parts[1]);
                    }
                }
            }
            else
            {
                EnregistrerNumeros();
            }
        }

        private void EnregistrerNumeros()
        {
            List<string> lignes = new List<string>();
            foreach (var type in compteurTypes)
            {
                lignes.Add($"{type.Key}:{type.Value}");
            }
            File.WriteAllLines(CheminFichierNumeros, lignes);
        }

        private void ChargerClients()
        {
            if (File.Exists(CheminFichierClients))
            {
                string[] lignes = File.ReadAllLines(CheminFichierClients);
                foreach (string ligne in lignes)
                {
                    string[] parts = ligne.Split(',');
                    if (parts.Length == 3)
                    {
                        clients.Add(new Client(parts[0], parts[1], parts[2]));
                    }
                }
            }
        }

        private void EnregistrerClients()
        {
            List<string> lignes = new List<string>();
            foreach (var client in clients)
            {
                lignes.Add($"{client.Nom},{client.Prenom},{client.NumeroTicket}");
            }
            File.WriteAllLines(CheminFichierClients, lignes);
        }

        public void AfficherMenu()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=================================================");
            Console.WriteLine("BIENVENUE DANS NOTRE DISTRIBUTEUR DE TICKETS BANCAIRE");
            Console.WriteLine("=================================================");
            Console.ResetColor();

            Console.WriteLine("Quel type d'opération souhaitez-vous effectuer ?");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("1. Versement");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("2. Retrait");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("3. Informations");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("4. Quitter");
            Console.ResetColor();
        }

        public bool DemanderTicket(string nom, string prenom)
        {
            // Saisie des informations du client si elles ne sont pas fournies
            string typeOperation = DemanderTypeOperation();

            compteurTypes[typeOperation]++;
            string numeroTicket = $"{typeOperation}-{compteurTypes[typeOperation]}";
            clients.Add(new Client(nom, prenom, numeroTicket));

            int personnesEnAttente = compteurTypes[typeOperation] - 1;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nVotre numéro est : {numeroTicket}");
            Console.WriteLine(personnesEnAttente == 0
                ? "Il n'y a personne en attente avant vous.\n"
                : $"Il y a {personnesEnAttente} personne(s) en attente avant vous.\n");
            Console.ResetColor();

            EnregistrerNumeros();
            EnregistrerClients(); // Enregistre également les informations des clients
            return true;
        }

        private string DemanderTypeOperation()
        {
            Console.Write("Veuillez sélectionner une option (1 à 4) : ");
            bool choixValide = false;
            string typeOperation = "";

            while (!choixValide)
            {
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        typeOperation = "V";
                        choixValide = true;
                        break;
                    case "2":
                        typeOperation = "R";
                        choixValide = true;
                        break;
                    case "3":
                        typeOperation = "I";
                        choixValide = true;
                        break;
                    case "4":
                        // Affichage des clients avant de quitter
                        AfficherClients();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nMerci d'avoir utilisé notre service. Appuyez sur une touche pour fermer le terminal.");
                        Console.ResetColor();
                        Console.ReadKey(); // Attendre que l'utilisateur appuie sur une touche
                        Environment.Exit(0);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Option invalide. Veuillez choisir un nombre entre 1 et 4 : ");
                        Console.ResetColor();
                        break;
                }
            }
            return typeOperation;
        }

        public bool DemanderUnAutreTicket()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Souhaitez-vous prendre un autre numéro ? (o pour Oui, n pour Non) : ");
            Console.ResetColor();

            while (true)
            {
                string reponse = Console.ReadLine()?.Trim().ToLower();

                if (reponse == "o" || reponse == "oui")
                {
                    return true; // Continue sans réafficher le menu
                }
                else if (reponse == "n" || reponse == "non")
                {
                    return false; // Réaffiche le menu
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Réponse invalide. Tapez 'o' pour Oui ou 'n' pour Non : ");
                    Console.ResetColor();
                }
            }
        }

        public void AfficherClients()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n==================== LISTE DES CLIENTS ====================");
            foreach (var client in clients)
            {
                Console.WriteLine(client);
            }
            Console.WriteLine("============================================================");
            Console.ResetColor();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DistributeurDeTickets distributeur = new DistributeurDeTickets();

            string nom = "";
            string prenom = "";

            while (true)
            {
                distributeur.AfficherMenu();

                // Demander les informations client une fois au début
                while (true)
                {
                    Console.Write("Veuillez entrer votre nom : ");
                    nom = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(nom))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Le nom ne peut pas être vide. Veuillez réessayer.");
                        Console.ResetColor();
                        continue;
                    }

                    Console.Write("Veuillez entrer votre prénom : ");
                    prenom = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(prenom))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Le prénom ne peut pas être vide. Veuillez réessayer.");
                        Console.ResetColor();
                        continue;
                    }

                    break; // Sort de la boucle si les saisies sont valides
                }

                distributeur.DemanderTicket(nom, prenom);

                // Si l'utilisateur veut un autre ticket, il continue sans afficher le menu
                while (distributeur.DemanderUnAutreTicket())
                {
                    distributeur.DemanderTicket(nom, prenom);
                }

                // Si l'utilisateur ne veut pas continuer, réinitialiser les informations
                nom = "";
                prenom = "";
            }
        }
    }
}
