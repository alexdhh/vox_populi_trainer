# 🗳️ Vox Populi 4.0 - AI Trainer & Generator

Vox Populi 4.0 est une application de bureau (développée avec .NET MAUI) conçue pour la création, l'entraînement et l'évaluation de modèles de Machine Learning spécialisés dans l'analyse du discours politique français (Gauche, Centre, Droite). 

L'application offre une interface moderne et fluide pour gérer l'intégralité du cycle de vie d'une IA d'analyse textuelle, de la génération de jeux de données synthétiques massifs jusqu'à l'exportation du modèle entraîné.

---

## ✨ Fonctionnalités Principales

### 1. Génération de Données Synthétiques (GenerateDataPage)
* **Matrices politiques :** Utilisation de matrices linguistiques (Sujet, Action, Cible, Objectif) pour générer des phrases politiquement connotées.
* **Hautes performances :** Algorithme de tirage optimisé (contournant le problème du collectionneur de vignettes) permettant de générer jusqu'à **18,75 millions de combinaisons uniques** sans saturation mémoire.
* **Export CSV :** Sauvegarde instantanée des données générées pour l'entraînement.

### 2. Entraînement de Modèles IA (CreateModelPage)
* **ML.NET Intégré :** Importation de datasets et entraînement direct depuis l'application.
* **Sélection d'algorithmes :** Choix parmi les meilleurs algorithmes de classification multiclasse de Microsoft (LbfgsRegressionOva, LightGbm, SdcaMaximumEntropy, FastTree, etc.).
* **Feedback visuel :** Barre de progression animée et estimation du temps d'entraînement.
* **Export complet :** Génération d'une archive .zip contenant le modèle binaire (.mlnet) ainsi que les classes C# d'évaluation et de consommation (.consumption.cs, .training.cs) prêtes à être intégrées.

### 3. Test des Modèles en Temps Réel (TestModelPage)
* **Import de modèle :** Chargement dynamique d'un modèle .mlnet précédemment entraîné.
* **Inférence Live :** Saisie de texte libre et prédiction immédiate du bord politique avec mise en évidence des résultats.

---

## 🛠️ Technologies & Architecture

* **Framework :** .NET MAUI (Multi-platform App UI)
* **Langage :** C# 11+
* **Architecture :** **MVVM** (Model-View-ViewModel) via le CommunityToolkit.Mvvm pour une séparation propre entre la logique métier et l'interface utilisateur.
* **Machine Learning :** ML.NET (Microsoft.ML)
* **UI/UX :** Design moderne avec coins arrondis, ombres douces, gradients et typographie personnalisée (TT Norms Pro).

---

## 🚀 Installation et Lancement

### Prérequis
* Visual Studio 2022 (ou supérieur)
* La charge de travail (workload) "Développement d'applications .NET MAUI" installée.

### Étapes
1. Clonez ce dépôt sur votre machine locale via votre terminal :
   git clone https://github.com/votre-nom-utilisateur/vox-populi-trainer.git

2. Ouvrez la solution "vox_populi_trainer.sln" dans Visual Studio.
3. Restaurez les packages NuGet si Visual Studio ne le fait pas automatiquement.
4. Sélectionnez "Windows Machine" (ou Mac Catalyst) comme cible de débogage.
5. Cliquez sur le bouton "Exécuter" (ou appuyez sur F5).

---

## 📁 Structure du Projet

* Models/ : Classes de données (ModelInput, ModelOutput pour ML.NET).
* ViewModels/ : Logique de l'application (Commandes, gestion d'état, algorithmes de génération et d'entraînement).
* Views/ : Interfaces utilisateurs en XAML (ChoicePage, GenerateDataPage, CreateModelPage, TestModelPage).
* Resources/ :
  * Fonts/ : Polices personnalisées (TT Norms Pro).
  * Images/ : Icônes et assets visuels de l'application.

---
