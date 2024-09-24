using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Crud_Serializzato
{
    public partial class Form1 : Form
    {
        private List<StudentVote> studentVotes = new List<StudentVote>();
        private string filePath = "StudentVotes.csv"; // Percorso file CSV
            
        public Form1()
        {
            InitializeComponent();
            LoadVotesFromFile(); // Carica i voti dal file all'avvio dell'applicazione
        }

        // Carica i voti dal file CSV
        private void LoadVotesFromFile()
        {
            if (File.Exists(filePath))
            {
                try
                {   
                    // Legge tutte le righe del file CSV e le memorizza in un array di stringhe
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        var values = line.Split(',');

                        // Verifica che ci siano esattamente 4 valori (nome, classe, materia, voto) e che l'ultimo valore sia un numero intero (il voto)
                        if (values.Length == 4 && int.TryParse(values[3], out int voteValue))
                        {
                            // Crea un oggetto StudentVote usando i valori ottenuti dalla riga
                            StudentVote vote = new StudentVote(values[0], values[1], values[2], voteValue);

                            // Aggiunge l'oggetto creato alla lista di voti (studentVotes)
                            studentVotes.Add(vote);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // In caso di errore durante la lettura del file, viene mostrato un messaggio con i dettagli dell'errore
                    MessageBox.Show("Errore durante il caricamento dei voti: " + ex.Message);
                }
            }
        }


        // Salva i voti nel file CSV
        private void SaveVotesToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var vote in studentVotes)
                    {
                        writer.WriteLine($"{vote.StudentName},{vote.ClassName},{vote.SubjectName},{vote.VoteValue}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il salvataggio dei voti: " + ex.Message);
            }
            BoxStudentName.Clear();
            BoxClassName.Clear();
            BoxSubjectName.Clear();
            BoxVoteValue.Clear();
            BoxModStudentName.Clear();

        }

        private void CreateVote_Click(object sender, EventArgs e)
        {
            string studentName = BoxStudentName.Text;
            string className = BoxClassName.Text;
            string subjectName = BoxSubjectName.Text;
            if (int.TryParse(BoxVoteValue.Text, out int voteValue))
            {
                StudentVote newVote = new StudentVote(studentName, className, subjectName, voteValue);
                studentVotes.Add(newVote);
                SaveVotesToFile(); // Salva il nuovo voto nel file CSV
                MessageBox.Show("Voto creato con successo!");
            }
            else
            {
                MessageBox.Show("Inserire un valore valido per il voto.");
            }
        }

        private void ReadVote_Click(object sender, EventArgs e)
        {
            // Verifica se ci sono voti nella lista
            if (studentVotes.Count > 0)
            {
                // Costruisce una stringa per visualizzare tutti i voti
                string allVotes = "Lista dei voti:\n";

                // Scorre l'intera lista di voti e aggiunge i dettagli di ciascun voto alla stringa
                foreach (var vote in studentVotes)
                {
                    allVotes += $"Studente: {vote.StudentName}, Classe: {vote.ClassName}, Materia: {vote.SubjectName}, Voto: {vote.VoteValue}\n";
                }

                // Mostra tutti i voti in un MessageBox
                MessageBox.Show(allVotes);
            }
            else
            {
                // Se non ci sono voti nella lista, informa l'utente
                MessageBox.Show("Nessun voto trovato.");
            }
        }

        private void UpdateVote_Click(object sender, EventArgs e)
        {
            // Prendi i nomi inseriti dall'utente
            string currentStudentName = BoxStudentName.Text;  // Nome corrente
            string newStudentName = BoxModStudentName.Text;   // Nuovo nome

            // Cerca nella lista il voto associato al nome corrente
                    StudentVote foundVote = studentVotes.Find(vote => vote.StudentName == currentStudentName);

            if (foundVote != null)
            {
                // Aggiorna il nome dello studente con il nuovo nome
                foundVote.StudentName = newStudentName;

                // Salva i cambiamenti nel file CSV
                SaveVotesToFile();

                // Notifica l'utente che l'aggiornamento è avvenuto con successo
                MessageBox.Show("Nome dello studente aggiornato con successo!");
            }
            else
            {
                // Se non viene trovato il voto, mostra un messaggio di errore
                MessageBox.Show("Nome dello studente non trovato.");
            }
        }

        private void DelateVote_Click(object sender, EventArgs e)
        {
            // Prendi il nome dello studente inserito dall'utente
            string studentNameToDelete = BoxStudentName.Text;

            // Cerca nella lista il voto associato al nome
            StudentVote foundVote = studentVotes.Find(vote => vote.StudentName == studentNameToDelete);

            if (foundVote != null)
            {
                // Rimuovi il voto dalla lista
                studentVotes.Remove(foundVote);

                // Salva i cambiamenti nel file CSV
                SaveVotesToFile();

                // Notifica l'utente che l'eliminazione è avvenuta con successo
                MessageBox.Show("Voto eliminato con successo!");
            }
            else
            {
                // Se non viene trovato il voto, mostra un messaggio di errore
                MessageBox.Show("Nome dello studente non trovato.");
            }
        }
        private void SearchVote_Click(object sender, EventArgs e)
        {
            // Verifica se l'input è un numero intero valido
            if (int.TryParse(BoxSearchVote.Text, out int searchVote))
            {
                // Trova tutti gli studenti con il voto specificato
                var studentsWithVote = studentVotes.Where(vote => vote.VoteValue == searchVote).ToList();

                // Verifica se ci sono studenti con il voto cercato
                if (studentsWithVote.Count > 0)
                {
                    // Costruisce una stringa con tutti gli studenti che hanno il voto cercato
                    string result = $"Studenti con voto {searchVote}:\n";
                    foreach (var student in studentsWithVote)
                    {
                        result += $"Studente: {student.StudentName}, Classe: {student.ClassName}, Materia: {student.SubjectName}\n";
                    }

                    // Mostra la lista degli studenti in una MessageBox
                    MessageBox.Show(result);
                }
                else
                {
                    // Se nessuno studente ha quel voto, mostra un messaggio
                    MessageBox.Show($"Nessuno studente ha il voto {searchVote}.");
                }
            }
            else
            {
                // Se l'input non è valido, mostra un messaggio di errore
                MessageBox.Show("Per favore, inserisci un voto valido.");
            }
        }



        private void EXIT_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
    public class StudentVote
    {
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public int VoteValue { get; set; }

        // Costruttore
        public StudentVote(string studentName, string className, string subjectName, int voteValue)
        {
            StudentName = studentName;
            ClassName = className;
            SubjectName = subjectName;
            VoteValue = voteValue;
        }
    }
}
