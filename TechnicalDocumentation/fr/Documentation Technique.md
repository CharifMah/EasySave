# Easy Save

## Introduction 

La branche livrable-1 contient la première version fonctionnelle d'EasySave, conformément au cahier des charges de ProSoft. 

Cette version initie le développement d'une application console en .NET Core pour la gestion de travaux de sauvegarde.

### Prérequis

1.	Installation process

* Clonez la branche livrable-1 et ouvrez le projet avec Visual Studio 2022. 
* Veillez à avoir .NET Core 6.X installé.

2.	Software dependencies

* .NET Core 6.X
* Visual Studio 2022

3.	Latest releases

Veuillez consulter l'onglet des releases pour accéder à la dernière version du livrable 1 (Pipelines -> Releases) :

https://dev.azure.com/faikmehmeti/G4-FISA-24/_release?_a=releases&view=mine&definitionId=1

### Build and Test

* Un pull request vers la branche develop-livrable-1 entraîne automatiquement un processus de build et de tests (tests unitaires uniquement).
* Un pull request vers la branche livrable-1 entraîne automatiquement un processus d'intégration continue, allant du build, aux tests, jusqu'à la publication d'une release.

Ces processus sont consultables via les pipelines : https://dev.azure.com/faikmehmeti/G4-FISA-24/_build

NOTES : Un seul agent pourra gérer l'éxécution d'une unique pipeline, le paralélisme n'est pas possible.

### Fonctionnalités
- Sauvegarde séquentielle
- Journaux quotidiens
- Journaux d'état
- Choisir la langue
- Lister les jobs
- Charger une configuration de Jobs
- Créer un job
- Supprimer un job
- Lancer les jobs

### Architecture
 L'architecture est basée sur un modèle MVVM avec :

- **Modèles** : classes représentant les données ([CJob](https://charifmah.github.io/EasySaveWiki/api/Models.Backup.CJob.html), [CJobManager](https://charifmah.github.io/EasySaveWiki/api/Models.Backup.CJobManager.html), [CSettings](https://charifmah.github.io/EasySaveWiki/api/Models.CSettings.html), etc.)
- **Vues** :  classes représentant les vues ([BaseView](https://charifmah.github.io/EasySaveWiki/api/EasySave.Views.BaseView.html), [View](https://charifmah.github.io/EasySaveWiki/api/EasySave.Views.View.html), [JobView](https://charifmah.github.io/EasySaveWiki/api/EasySave.Views.JobView.html), etc.)
- **ViewModels** : classes faisant le lien entre modèles et vues.


