using CommunityToolkit.Maui.Storage;

namespace vox_populi_trainer.ViewModels
{
    public partial class GenerateDataPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial string SampleCountInput { get; set; }

        [ObservableProperty]
        public partial bool IsGenerating { get; set; }

        [ObservableProperty]
        public partial bool IsGenerationCompleted { get; set; }

        [ObservableProperty]
        public partial string StatusText { get; set; }

        [ObservableProperty]
        public partial double Progress { get; set; }

        [ObservableProperty]
        public partial string ProgressPercentage { get; set; }

        private string _generatedFilePath;

        public GenerateDataPageViewModel()
        {
            SampleCountInput = "100000";
            IsGenerating = false;
            IsGenerationCompleted = false;
        }

        [RelayCommand]
        private async Task ReturnAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChoicePage));
        }

        [RelayCommand]
        private async Task StartGeneration()
        {
            if (!int.TryParse(SampleCountInput, out int sampleCount) || sampleCount <= 0)
            {
                StatusText = "Veuillez entrer un nombre valide supérieur à 0.";
                return;
            }

            // La matrice fait 50x50x50x50 = 6 250 000 combinaisons max
            if (sampleCount > 6250000)
            {
                sampleCount = 6250000;
                SampleCountInput = "6250000";
            }

            IsGenerating = true;
            IsGenerationCompleted = false;
            Progress = 0;
            ProgressPercentage = "0%";
            StatusText = "Initialisation des matrices...";

            await Task.Run(() => GenerateDataset(sampleCount));
        }

        private void GenerateDataset(int sampleCount)
        {
            try
            {
                string[] sujets_g = { "L'État", "Le gouvernement", "La République", "La puissance publique", "La nation", "Notre société", "La collectivité", "La gauche", "Le secteur public", "L'interventionnisme d'État", "La solidarité nationale", "L'action publique", "La représentation nationale", "Notre démocratie", "L'organisation sociale", "La transition écologique", "Le pouvoir public", "Le pays", "La communauté nationale", "La justice sociale", "Le peuple", "L'union des gauches", "Le législateur progressiste", "Le service public", "L'administration", "La sphère publique", "L'appareil d'État", "Le corps social", "L'intelligence collective", "Le mouvement social", "Le front populaire", "L'avant-garde citoyenne", "Le syndicalisme", "La société civile", "L'écosystème public", "L'appareil d'État social", "La régulation citoyenne", "La force publique", "Le bloc de gauche", "L'idéal républicain", "Le pacte social", "L'ensemble des travailleurs", "La classe ouvrière", "La jeunesse engagée", "La voix du peuple", "L'initiative publique", "L'instance de régulation", "Le pouvoir populaire", "La majorité sociale", "L'horizon écologique" };
                string[] actions_g = { "doit financer massivement", "a le devoir de revaloriser", "se doit de renforcer", "doit investir dans", "a pour obligation de protéger", "doit augmenter", "se doit de garantir", "doit développer fortement", "a la responsabilité de soutenir", "doit sanctuariser", "se doit d'étendre", "doit promouvoir", "a le mandat de pérenniser", "doit démocratiser", "a l'impératif de sauver", "doit subventionner", "doit réguler strictement", "se doit d'encadrer", "doit nationaliser", "a la charge d'améliorer", "ne doit jamais abandonner", "ne peut plus négliger", "ne doit pas sacrifier", "ne doit en aucun cas délaisser", "ne peut plus ignorer la détresse de", "ne doit pas privatiser", "ne doit jamais céder sur", "ne peut plus tolérer la baisse de", "ne doit pas marchander", "ne doit en aucun cas réduire", "doit réorienter massivement", "se doit de planifier", "a l'urgence d'imposer", "doit reprendre en main", "ne peut plus brader", "ne doit en aucun cas précariser", "doit collectiviser", "doit socialiser", "se doit de consolider", "doit redéployer", "ne doit jamais livrer au marché", "a la mission historique de défendre", "doit financer sans condition", "ne peut plus laisser dépérir", "doit urgemment secourir", "doit accompagner activement", "se doit de piloter", "doit revitaliser", "ne doit plus sous-financer", "doit abriter" };
                string[] cibles_g = { "les services publics", "l'hôpital et l'école", "le système de santé", "les aides sociales", "la transition écologique", "le logement social", "les minimas sociaux", "le droit du travail", "l'éducation nationale", "l'assurance maladie", "les retraites par répartition", "l'accueil des réfugiés", "le pouvoir d'achat des plus modestes", "la gratuité des transports", "le maillage associatif", "la taxation des super-profits", "le partage de la valeur", "les énergies renouvelables", "l'égalité salariale", "la prime d'activité", "les biens communs", "l'accès à l'eau", "le budget de la recherche", "les conditions de travail", "la régulation écologique", "l'impôt sur la fortune", "la dignité humaine", "les droits sociaux", "l'économie sociale et solidaire", "la justice fiscale", "la planification écologique", "l'industrie verte", "les infrastructures publiques", "le fret ferroviaire", "l'accès au logement", "les salaires de base", "les métiers du lien", "la sécurité sociale de l'alimentation", "le revenu étudiant", "les caisses de retraite", "les droits syndicaux", "le pôle public de l'énergie", "la protection maternelle", "le tissu associatif local", "l'indépendance de la presse", "les crèches publiques", "l'hébergement d'urgence", "le système éducatif", "la biodiversité", "le droit à l'eau potable" };
                string[] objectifs_g = { "pour réduire les inégalités.", "afin de protéger les plus vulnérables.", "pour garantir la justice sociale.", "dans le but d'éradiquer la précarité.", "pour assurer une vraie solidarité nationale.", "afin de lutter contre l'exclusion.", "pour le bien-être de tous.", "dans le but de protéger la planète.", "pour construire un avenir plus juste.", "afin d'aider les travailleurs.", "pour combattre la pauvreté.", "afin de redistribuer les richesses.", "pour que personne ne soit laissé pour compte.", "dans un souci d'équité sociale.", "pour émanciper les citoyens.", "afin de préserver notre écosystème.", "pour taxer les ultra-riches.", "dans le but de plafonner les loyers.", "pour en finir avec l'évasion fiscale.", "afin de partager le temps de travail.", "pour ne laisser personne dans la misère.", "afin de ne pas céder au grand capital.", "pour éviter la casse sociale.", "afin de ne pas détruire notre environnement.", "pour stopper l'hémorragie des services publics.", "afin de ne jamais oublier les plus faibles.", "pour mettre fin à la loi du plus fort.", "afin d'empêcher la marchandisation du monde.", "pour instaurer une véritable égalité.", "afin de reprendre le pouvoir au marché.", "pour réparer le tissu social.", "afin d'isoler les logements passoires.", "pour faire payer les pollueurs.", "afin de sécuriser les parcours professionnels.", "pour empêcher l'enrichissement indécent.", "afin d'enrayer la machine capitaliste.", "pour redonner de l'espoir aux jeunes.", "afin de bâtir la société de demain.", "pour ne plus subir la finance.", "afin d'assurer la dignité des plus fragiles.", "pour mettre l'humain avant le profit.", "afin d'en finir avec les privilèges.", "pour refonder notre contrat social.", "afin de garantir l'accès aux soins partout.", "pour que la rue ne soit plus une fatalité.", "afin d'éradiquer les déserts médicaux.", "pour promouvoir le progrès social.", "afin d'interdire les licenciements boursiers.", "pour sanctuariser les biens communs.", "afin de respecter les accords climatiques." };

                string[] sujets_d = { "L'État", "Le gouvernement", "La République", "Notre pays", "La nation", "L'exécutif", "Le pouvoir politique", "Les dirigeants", "La France", "Notre démocratie", "La droite", "Le législateur", "L'autorité publique", "Les institutions", "Notre patrie", "Le libre marché", "L'entreprise", "L'initiative privée", "La compétitivité", "L'ordre républicain", "Le chef de l'État", "Le patronat", "Le ministère de l'Intérieur", "Les forces de l'ordre", "La justice", "Le gouvernement responsable", "Le monde de l'entreprise", "Le secteur privé", "L'ordre public", "Le patriotisme économique", "Le bloc souverainiste", "L'appareil d'État régalien", "La majorité silencieuse", "Le monde rural", "Le contribuable", "Le ministère de l'Économie", "Le garant de la sécurité", "Le rempart républicain", "Le socle national", "L'identité française", "La tradition", "Le secteur productif", "La droite républicaine", "L'alliance des droites", "Le bon sens populaire", "Le camp de la fermeté", "Le parti de l'ordre", "L'union nationale", "La justice de notre pays", "La cour des comptes" };
                string[] actions_d = { "doit réduire impérativement", "a l'obligation de baisser", "se doit d'alléger", "doit diminuer", "a pour devoir de limiter", "doit réformer en profondeur", "se doit de maîtriser", "doit couper dans", "a la responsabilité de contrôler", "doit freiner", "se doit de rationaliser", "a l'exigence de baisser", "doit stopper", "a l'impératif de réguler", "doit abolir", "doit libéraliser", "se doit de privatiser", "doit durcir", "a le mandat de sécuriser", "doit expulser", "ne doit plus supporter", "ne peut plus accepter", "ne doit en aucun cas augmenter", "ne doit pas entraver", "ne doit plus financer l'assistanat de", "ne peut plus tolérer", "ne doit jamais taxer davantage", "ne doit pas alourdir", "ne doit pas complexifier", "ne doit plus subir", "doit déréglementer", "se doit de sanctionner", "doit baisser drastiquement", "a l'urgence d'endiguer", "ne doit plus encourager", "doit flexibiliser", "se doit d'optimiser", "doit restructurer", "ne doit jamais tolérer", "a le devoir de redresser", "doit purger", "ne peut plus cautionner", "doit alléger massivement", "doit cesser de taxer", "se doit de rétablir", "doit pacifier", "ne doit pas subventionner", "a la mission de protéger", "doit décentraliser", "ne doit plus brider" };
                string[] cibles_d = { "la pression fiscale", "les charges patronales", "les impôts sur les entreprises", "les dépenses de fonctionnement", "le poids de l'administration", "l'assistanat", "les normes abusives", "la dette publique", "les prélèvements obligatoires", "la bureaucratie", "l'immigration illégale", "le laxisme judiciaire", "les taxes inutiles", "le gaspillage de l'argent public", "les aides non financées", "le code du travail", "l'insécurité", "les frontières", "la fraude sociale", "les délinquants", "l'assistanat généralisé", "l'insécurité galopante", "les impôts punitifs", "les normes environnementales strictes", "l'immigration clandestine", "les aides sociales abusives", "la fraude fiscale et sociale", "les contraintes administratives", "la dépense publique", "le millefeuille administratif", "les syndicats bloquants", "le wokisme", "l'insécurité culturelle", "les subventions aux associations", "l'impôt sur la succession", "l'assistanat chronique", "les normes européennes", "le poids de la fiscalité", "le coût du travail", "l'immigration de peuplement", "les squats et occupations illégales", "le communautarisme", "la délinquance des mineurs", "les aides médicales d'état", "les dépenses sociales", "la fraude à la carte vitale", "les revendications minoritaires", "les grèves à répétition", "le monopole d'état", "l'inflation normative" };
                string[] objectifs_d = { "pour relancer la croissance.", "afin de restaurer notre compétitivité.", "pour valoriser le mérite et le travail.", "dans le but de libérer l'économie.", "pour rétablir l'autorité de l'État.", "afin de soutenir nos entreprises.", "pour garantir notre souveraineté.", "dans le but d'assainir les finances publiques.", "pour encourager l'initiative privée.", "afin de récompenser l'effort.", "pour défendre nos valeurs traditionnelles.", "afin de protéger notre identité nationale.", "pour que le travail paie plus que l'assistance.", "dans le but de restaurer l'ordre.", "pour sécuriser l'avenir économique.", "afin de stopper l'immigration massive.", "pour désendetter la France.", "dans le but de punir sévèrement les criminels.", "pour rétablir la compétitivité coût.", "afin d'encourager les créateurs de richesse.", "pour ne pas étouffer les entreprises.", "afin de ne jamais céder face au désordre.", "pour éviter la faillite de l'État.", "afin de ne plus décourager le travail.", "pour stopper le déclin de la France.", "afin de ne pas subir la loi des minorités.", "pour arrêter de pénaliser ceux qui réussissent.", "afin de ne plus vivre à crédit.", "pour récompenser la France qui se lève tôt.", "afin de restaurer l'autorité dans les écoles.", "pour défendre la civilisation.", "afin de stopper le grand déclassement.", "pour remettre la France au travail.", "afin de ne plus être la vache à lait de l'Europe.", "pour libérer les énergies productives.", "afin d'en finir avec l'angélisme pénal.", "pour protéger les honnêtes gens.", "afin de stopper l'hémorragie financière.", "pour garantir l'intégrité du territoire.", "afin de redonner le goût de l'effort.", "pour stopper l'assistanat d'État.", "afin de ne plus punir la réussite.", "pour recréer des champions industriels.", "afin de simplifier la vie des entrepreneurs.", "pour retrouver notre fierté nationale.", "afin de dégraisser le mammouth administratif.", "pour couper les vivres aux profiteurs.", "afin de protéger le modèle familial traditionnel.", "pour fermer les frontières passoires.", "afin de garantir la sécurité au quotidien." };

                MainThread.BeginInvokeOnMainThread(() => StatusText = "Échantillonnage et mélange...");

                int totalCombinations = 50 * 50 * 50 * 50;
                int totalDatasetSize = sampleCount * 2;

                var datasetIndices = new List<(int Index, bool IsGauche)>(totalDatasetSize);

                var random = new Random();
                var indicesG = GenerateUniqueRandomIndices(sampleCount, totalCombinations, random);
                var indicesD = GenerateUniqueRandomIndices(sampleCount, totalCombinations, random);

                foreach (var idx in indicesG) datasetIndices.Add((idx, true));
                foreach (var idx in indicesD) datasetIndices.Add((idx, false));

                for (int i = datasetIndices.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    var temp = datasetIndices[i];
                    datasetIndices[i] = datasetIndices[j];
                    datasetIndices[j] = temp;
                }

                string localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "synthetic_data.csv");

                using (var writer = new StreamWriter(localPath, false, System.Text.Encoding.UTF8))
                {
                    for (int i = 0; i < datasetIndices.Count; i++)
                    {
                        var item = datasetIndices[i];
                        int index = item.Index;

                        int o_idx = index % 50; index /= 50;
                        int c_idx = index % 50; index /= 50;
                        int a_idx = index % 50; int s_idx = index / 50;

                        string phrase;
                        string label = item.IsGauche ? "Gauche" : "Droite";

                        if (item.IsGauche)
                            phrase = $"{sujets_g[s_idx]} {actions_g[a_idx]} {(c_idx < 50 ? cibles_g[c_idx] : "")} {(o_idx < 50 ? objectifs_g[o_idx] : "")}";
                        else
                            phrase = $"{sujets_d[s_idx]} {actions_d[a_idx]} {(c_idx < 50 ? cibles_d[c_idx] : "")} {(o_idx < 50 ? objectifs_d[o_idx] : "")}";

                        // La modification est ici : on a retiré les guillemets et le .Replace()
                        writer.WriteLine($"{phrase},{label}");

                        if (i % 10000 == 0)
                        {
                            double currentProgress = (double)i / totalDatasetSize;
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Progress = currentProgress;
                                ProgressPercentage = $"{(int)(currentProgress * 100)}%";
                                StatusText = $"Génération en cours : {i:N0} / {totalDatasetSize:N0} phrases...";
                            });
                        }
                    }
                }

                _generatedFilePath = localPath;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Progress = 1.0;
                    ProgressPercentage = "100%";
                    StatusText = $"Génération terminée ({totalDatasetSize:N0} lignes)";
                    IsGenerationCompleted = true;
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() => StatusText = $"Erreur : {ex.Message}");
            }
        }

        private HashSet<int> GenerateUniqueRandomIndices(int count, int maxExclusive, Random rnd)
        {
            var indices = new HashSet<int>(count);
            while (indices.Count < count)
            {
                indices.Add(rnd.Next(maxExclusive));
            }
            return indices;
        }

        [RelayCommand]
        private async Task ExportDataset(CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(_generatedFilePath) || !File.Exists(_generatedFilePath)) return;

                using var stream = new FileStream(_generatedFilePath, FileMode.Open, FileAccess.Read);

                var fileSaverResult = await FileSaver.Default.SaveAsync("training_data.csv", stream, cancellationToken);
            }
            catch (Exception ex)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", $"Impossible d'exporter : {ex.Message}", "Fermer");
                }
            }
        }
    }
}