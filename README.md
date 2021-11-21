# Projet Architecture logiciel

Application API web en micro services. Elle utilise une bibliothèque qui automatise les standards d'une API REST.

#### La base
* Librairie impléte par défaut un CRUD (Create, Read, Update, Delete),  
* un tri, une pagination et un rendu partiel. Lors du développement de l'API à partir de la librairie, 

#### Pagination
La pagination en utilisant le paramètre de requête ?range=0-25 et les Header standards HTTP pour la réponse: Content-Range & Accept-Range. Voici comment les links sont présent dans le header de retour: 
* rel="first" : https://xxxxx.com/catalog/v1/products/orders?range=0-7; 
* rel="prev" : https://xxxxx.com/catalog/v1/products/orders?range=40-47; 
* rel="next" : https://xxxxx.com/catalog/v1/products/orders?range=56-64; 
* rel="last" : https://api.xxxxx.com/catalog/v1/products/orders?range=968-975; 

Voici ce que contient également le header dans le cas d'une pagination: Content-Range: 0-47/48 Accept-Range: product 50

##### Tris
Le tri du résultat d’un appel sur une collection de ressources passe par deux principaux paramètres :

sort : contient les noms des attributs, séparés par une virgule, sur lesquels effectuer le trie. desc : par défaut le tri est ascendant (ou croissant), afin de l’obtenir de façon descendant (ou décroissant), il suffit d’ajouter ce paramètre (sans valeur par défaut). On voudra dans certains cas spécifier quels attributs doivent être traités de façon ascendant ou descendant, on mettra alors dans ce paramètre la liste des attributs descendants, les autres seront donc par défaut ascendants.

Le tri est sous la forme suivante: https://xxxxx.com/catalog/v1/products?asc=rating&desc=name

#### Filtres
La librairie inclus un filtre générique sous la forme suivante: http://xxxxx.com/catalog/v1/products?type=pizza,pates&rating=4,5&days=sunday Sur une chaine de caratères:

* l'utilisateur peut rechercher une valeur fixe (type=pizza)
* l'utilisateur peut rechercher plusieurs valeurs, ex les produits de type 'pizza' ou 'pates' (type=pizza,pate)
Sur les valeurs numériques:

* l'utilisateur peut rechercher une valeur fixe (rating=4)
* l'utilisateur peut rechercher plusieurs valeurs, ex les produits de rating '4' ou '5' (rating=4,5)
* l'utilisateur peut rechercher des fourchettes de valeurs, ex les produits de rating compris en '4' et '10' (rating=[4,10])
* l'utilisateur peut rechercher des valeurs inférieurs ou égal, ex les produits de rating inférieur ou egal à '10' (rating=[,10])
* l'utilisateur peut rechercher des valeurs supérieurs ou égal, ex les produits de rating séperieur ou égal à '4' (rating=[4,])
Sur les valaurs de temps:

* l'utilisateur peut rechercher une valeur fixe (createdat=04-04-2020)
* l'utilisateur peut rechercher plusieurs valeurs, ex les produits créés le 04/04/2020 ou le 05/05/2020 (createdat=04-04-2020,05-05-2020)
* l'utilisateur peut rechercher des fourchettes de valeurs, ex les produits créés entre le 04/04/2020 et le 05/05/2020 (createdat=[04-04-2020,05-05-2020])
* l'utilisateur peut rechercher des valeurs inférieurs ou égal, ex les produits créés avant le 05/05/2020 (createdat=[,05-05-2020])
* l'utilisateur peut rechercher des valeurs supérieurs ou égal, ex les produits créés après le 04/04/2020 (createdat=[04-04-2020,])

#### Recherche
La librairie inclus une recherche générique sous la forme suivante: http://xxxxx/catalog/v1/products/search?name=*napoli*&type=pizza,pate&sort=rating,name

#### Base de données
La base de données de dev est hébergée directement sur une instance Azure.

#### Web API
La doc de l'API est avec SwaggerUI aide.
