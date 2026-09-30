# Calculatrice MAUI

Application mobile de calculatrice développée avec .NET MAUI (Single Project),
réalisée dans le cadre de l'Activité N°4 - Atelier de développement Mobile.

## Fonctionnalités
- Addition, soustraction, multiplication, division
- Saisie de nombres décimaux
- Remise à zéro totale (AC) et effacement du dernier caractère (⌫)
- Changement de signe (±) et calcul de pourcentage (%)
- Gestion de la division par zéro sans plantage de l'application
- Affichage de l'opération en cours au-dessus du résultat

## Layouts utilisés (au moins 4 imposés)
- **ScrollView** : évite tout débordement de contenu sur petit écran ou au changement d'orientation.
- **Grid** (x2, imbriquées) : structure la page en zone d'affichage / clavier, puis organise le clavier en lignes et colonnes.
- **VerticalStackLayout** : empile le libellé de l'opération en cours au-dessus du résultat.
- **HorizontalStackLayout** : aligne la rangée de fonctions (AC, ⌫, %, ÷).
- **Border** : encadre l'écran d'affichage avec des coins arrondis.

## Ouvrir le projet dans VS Code
1. Installer la charge de travail .NET MAUI : `dotnet workload install maui`
2. Ouvrir le dossier du projet dans VS Code avec l'extension **.NET MAUI**.
3. Sélectionner la cible de déploiement (Android emulator, Windows, etc.).
4. Lancer avec F5 ou `dotnet build -t:Run -f net8.0-android`.
