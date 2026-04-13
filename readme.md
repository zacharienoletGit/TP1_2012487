# Développement d’un jeu interactif en réalité virtuelle avec Unity

## Auteur
Zacharie Nolet  
Techniques de l’informatique  
Cégep de Victoriaville  

---

## Contexte

Ce projet a été réalisé dans le cadre du cours portant sur le développement d’environnements immersifs en réalité virtuelle. Il vise à mettre en pratique les notions vues en classe, notamment l’utilisation de Unity et des outils XR.

Le projet consiste en la création d’un jeu simple inspiré du principe « Whack-a-Mole », où l’utilisateur doit frapper des cibles apparaissant dans un environnement 3D.

---

## Objectifs du projet

- Mettre en place un environnement VR fonctionnel
- Comprendre l’utilisation du XR Interaction Toolkit
- Implémenter des interactions physiques (collision, manipulation)
- Structurer un projet Unity avec des scripts en C#
- Ajouter des éléments de rétroaction (son)

---

## Technologies utilisées

- Unity
- C#
- XR Interaction Toolkit
- OpenXR
- Meta Quest (simulation)

---

## Description du fonctionnement

L’utilisateur est placé dans un environnement virtuel. Des cibles apparaissent à différents endroits.

À l’aide d’un marteau interactif, le joueur doit frapper ces cibles.

Lorsqu’une collision est détectée :

- la cible est supprimée
- un son est joué
- le score est augmenté

Le système repose sur la détection de collision et sur un gestionnaire central du jeu.

---

## Interactions

- Manipulation d’objets en VR
- Détection de collisions
- Effets sonores lors des impacts
- Interaction en temps réel avec l’environnement

---

## Tutoriel d’utilisation

### Lancer le projet

1. Ouvrir le projet dans Unity
2. Vérifier que OpenXR est activé :
   - Project Settings → XR Plug-in Management → OpenXR
3. S’assurer que les profils de contrôleurs sont activés
4. Lancer la scène principale

---

### Jouer

1. Mettre le casque VR (ou utiliser la simulation)
2. Utiliser les contrôleurs pour interagir
3. Prendre le marteau avec le bouton de prise (trigger)
4. Frapper les cibles lorsqu’elles apparaissent
5. Observer le score augmenter

---

### Fonctionnement technique simplifié

- Les cibles possèdent un collider
- Le marteau possède un collider et un Rigidbody
- Lors d’une collision :
  - le script détecte l’objet
  - vérifie le tag « Hammer »
  - appelle le GameManager
  - détruit la cible

