https://charifmah.github.io/EasySaveWiki/index.html
https://charifmah.github.io/EasySaveWiki/releases/Releases-v1-0/en/release-notes-en-1.0.0.html
https://charifmah.github.io/EasySaveWiki/TechnicalDocumentation/en/Technical%20Documentation.html
https://charifmah.github.io/EasySaveWiki/User-Guide/en/user-guid.html

![EasySaveGUI](https://github.com/user-attachments/assets/3efef8e8-2276-430c-87f1-c2779d24028a)

# Introduction 

La branche livrable-1 contient la première version fonctionnelle d'EasySave, conformément au cahier des charges de ProSoft. 

Cette version initie le développement d'une application console en .NET Core pour la gestion des travaux de sauvegarde.

# Prérequis

1.	Installation process

* Clonez la branche livrable-1 et ouvrez le projet avec Visual Studio 2022. 
* Assurez-vous que .NET Core 6.X est installé sur votre machine.

2.	Software dependencies

* .NET Core 6.X
* Visual Studio 2022

3.	Latest releases

Veuillez consulter l'onglet Releases pour accéder à la dernière version du livrable 1 (Pipelines -> Releases) :

https://dev.azure.com/faikmehmeti/G4-FISA-24/_release?_a=releases&view=mine&definitionId=1

# Build and Test

* Un pull request vers la branche develop-livrable-1 entraîne automatiquement un processus de build et de tests (tests unitaires uniquement).
* Un pull request vers la branche livrable-1 entraîne automatiquement un processus d'intégration continue, allant du build, aux tests, jusqu'à la publication d'une release.

Ces processus sont consultables via des pipelines : https://dev.azure.com/faikmehmeti/G4-FISA-24/_build

NOTES : Un seul agent pourra gérer l'éxécution d'un seul pipeline, le parallélisme n'étant pas possible.

# Contribute

Pour contribuer à cette branche, voici le processus à suivre et les nommages à respecter :

## Depuis Boards (Menu latéral)

1. Dans backlogs : 

    * Ajout d'un User Story (Fonctionnalité) -> Button (New Work Item) -> Title: description ...
    * Ajout d'une tâche fixbug -> Button (New Work Item) -> Title: description...

2. Dans boards (Tableau Kanban) :
        
    * Mettre à jour l'état de la tache -> Boards (Tableau KanBan : To Do, Doing, Done)
    * Active (Fonctionnalité en cours de développement), Closed (Fonctionnalité fini).
    * Ne pas mettre la card sur l'état resolved pour garder une trace, sinon elle disparaîtra.

3. Work items :
    
    * Aller : sur la tâche que vous voulez effectuer
    * Pour associer : une branche à la tâche : create a branch
    * Nom de la branche : feature/NumTache-description-courte ou fixbug/NumTache-description-courte
    * Based on : Toujours tirer la branche depuis develop-livrable-1

4. Push/Pull Request/Merge Conflicts on Develop :

    * Lors du push, notif : Create a pull request
    * Into : develop-livrable-1
    * Mettre une description détaillée
    * Attente du merge conflicts + Pipeline Build & Test
    * Complete PR -> Complete merge


5. Une fois la PR mergée sur Azure Devops, mettre à jour la branche livrable-1 si tout fonctionne

* En local :

    * git checkout develop-livrable-1
    * git pull origin develop-livrable-1 (Pour récup les PR mergées)
    * Tester sur l'application si tout marche

## Depuis Repos (Menu latéral)

* Sur azure devops :

    * Créer une nouvelle PR (onglet Pull requests) avec cette fois into livrable-1
    * Attente du merge conflicts + Pipeline Build & Test & Push Release
    * !!! Attention lors du merge validé, il faut décocher "supprimer cette branche" !!!
