using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyskleyTech.Math.Matrix
{
    using System.Globalization;
    using Nombre = System.Double;
    public class Matrix
    {
        //private double[,] matrice;
        private Nombre[,] matrix;
        //Cree une matrice a partir d'une taille
        public Matrix(int line , int cols)
        {
            matrix = new Nombre[line , cols];
        }
        //Copie une matrice
        public Matrix(Nombre[ , ] matrix)
        {
            this.matrix = ( Nombre[ , ] ) matrix.Clone();
        }
        /// <summary>
        /// Addition matricielle
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Matrix Add(Matrix b)
        {
            if ( !( this.Colonnes == b.Colonnes && this.Lignes == b.Lignes ) )
                throw new SizeMismatchException("Les deux matrices n'ont pas la meme dimmension");
            Matrix c = new Matrix(Lignes,Colonnes);
            for ( var i = 0 ; i < this.Lignes ; i++ )
            {
                for ( var j = 0 ; j < this.Colonnes ; j++ )
                {
                    c[i , j] = this[i , j] + b[i , j];
                }
            }
            return c;
        }
        /// <summary>
        /// La méthode de Gauss est défini au niveau de la matrice pour être plus éfficace au changement de contenu
        /// </summary>
        /// <returns>Matrice simplifié</returns>
        public Matrix Gauss()
        {
            string s="";
            return Gauss(out s);
        }
        /// <summary>
        /// La méthode de gauss avec un string de sortie
        /// </summary>
        /// <param name="s">Sortie des étapes</param>
        /// <returns>Matrice simplifié</returns>
        public Matrix Gauss(out string s)
        {
            s = "";
            Matrix res = Clone();
            Matrix operation = new Matrix(Lignes,Colonnes+1);

            s += res + "\r\n";
            //Pour mettre des 0 en bas des pivots
            for ( var lignePivot = 0 ; lignePivot < res.Lignes - 1 ; lignePivot++ )
            {
                //Trouver le pivot pour la ligne
                int positionPivot = res.gaussFindPivotForLine(lignePivot);
                if ( positionPivot == -1 )
                    throw new InvalidOperationException("Impossible de trouver un pivot");
                operation *= 0;
                for ( var ligne = lignePivot + 1 ; ligne < res.Lignes ; ligne++ )
                {
                    Nombre facteur = res.matrix[ligne,positionPivot]/res.matrix[lignePivot,positionPivot];
                    operation[ligne , Colonnes] = facteur;
                    for ( var colonne = 0 ; colonne < res.Colonnes ; colonne++ )
                    {
                        operation[ligne , colonne] = -res.matrix[lignePivot , colonne] * facteur;
                        res.matrix[ligne , colonne] = res.matrix[lignePivot , colonne] * facteur - res.matrix[ligne , colonne];
                    }
                }

                s += "Operation : " + operation.ToString(true) + "\r\n";
                s += "Matrice : " + res.ToString(true) + "\r\n";
            }
            //Pour mettre des 0 en haut des pivots et mettre la diagonale à 1
            for ( var lignePivot = res.Lignes - 1 ; lignePivot >= 0 ; lignePivot-- )
            {
                int positionPivot = res.gaussFindPivotForLine(lignePivot);
                if ( positionPivot == -1 )
                    throw new InvalidOperationException("Impossible de trouver un pivot");
                operation *= 0;

                Nombre facteurDiv = res.matrix[lignePivot,positionPivot];
                operation[lignePivot , Colonnes] = facteurDiv;
                for ( var j = 0 ; j < res.Colonnes ; j++ )
                {
                    operation[lignePivot , j] = facteurDiv;
                    res.matrix[lignePivot , j] /= facteurDiv;
                }

                for ( var ligne = lignePivot - 1 ; ligne >= 0 ; ligne-- )
                {
                    Nombre facteur = res.matrix[ligne,positionPivot];
                    operation[ligne , Colonnes] = facteur;
                    for ( var colonne = 0 ; colonne < res.Colonnes ; colonne++ )
                    {
                        operation[ligne , colonne] = -res.matrix[lignePivot , colonne] * facteur;
                        res.matrix[ligne , colonne] = res.matrix[lignePivot , colonne] * facteur - res.matrix[ligne , colonne];
                    }
                }

                s += "Operation : " + operation.ToString(true) + "\r\n";
                s += "Matrice : " + res.ToString(true) + "\r\n";
            }

            return res;
        }
        /// <summary>
        /// Permet d'augmenter une matrice pour permettre gauss
        /// </summary>
        /// <param name="b">Matrice B</param>
        /// <returns>Matrice augmentée</returns>
        public Matrix Augment(Matrix b)
        {
            Matrix a = this;
            if ( a.Lignes != b.Lignes )
                throw new SizeMismatchException("Les deux matrices n'ont pas le meme nombre de ligne");
            Matrix c = new Matrix(a.Lignes, a.Colonnes+b.Colonnes);
            for ( var i = 0 ; i < a.Lignes ; i++ )
            {
                for ( var j = 0 ; j < a.Colonnes ; j++ )
                {
                    c[i , j] = a[i , j];
                }
                for ( var j = 0 ; j < b.Colonnes ; j++ )
                {
                    c[i , j + a.Colonnes] = b[i , j];
                }
            }
            return c;
        }
        /// <summary>
        /// Permet d'identifier un pivot de gauss pour une ligne
        /// </summary>
        /// <param name="lignePivot">Numéro de ligne</param>
        /// <returns>Numéro de colonne du pivot</returns>
        private int gaussFindPivotForLine(int lignePivot)
        {
            for ( var i = 0 ; i < Colonnes ; i++ )
            {
                if ( matrix[lignePivot , i] != 0 )
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Produit entre matrice et nombre
        /// </summary>
        /// <param name="b">Le nombre</param>
        /// <returns>Matrice résultante</returns>
        public Matrix ScalarProduct(Nombre b)
        {
            Matrix c = new Matrix(Lignes,Colonnes);
            for ( var i = 0 ; i < this.Lignes ; i++ )
            {
                for ( var j = 0 ; j < this.Colonnes ; j++ )
                {
                    c[i , j] = this[i , j] * ( Nombre ) b;
                }
            }
            return c;
        }
        /// <summary>
        /// Fait un produit de 2 matrices
        /// </summary>
        /// <param name="b">Deuxième matrice à multiplier</param>
        /// <returns>Combinaison Résultat et nombre d'opération</returns>
        public Duo<Matrix , int> Product(Matrix b)
        {
            int operationCount = 0;
            if ( this.Colonnes != b.Lignes )
                throw new SizeMismatchException("Le nombre de ligne de la matrice A n'est pas égal au nombre de colonne de la matrice B");
            Matrix c = new Matrix(this.Lignes,b.Colonnes);

            for ( int i = 0 ; i < c.Lignes ; i++ )
            {
                for ( int j = 0 ; j < c.Colonnes ; j++ )
                {
                    for ( int d = 0 ; d < this.Colonnes ; d++ )
                    {
                        c[i , j] += this[i , d] * b[d , j];
                        operationCount++;
                    }
                }
            }
            return new Duo<Matrix , int>(c , operationCount);
        }
        /// <summary>
        /// Calcul le nombre d'opération pour un produit par rapport à une matrice B
        /// </summary>
        /// <param name="b">Deuxième matrice</param>
        /// <returns>Nombre opération</returns>
        public int ProductOperationCount(Matrix b)
        {
            var operationCount = 0;
            if ( this.Colonnes != b.Lignes )
                throw new SizeMismatchException("Le nombre de ligne de la matrice A n'est pas égal au nombre de colonne de la matrice B");

            for ( int ligne = 0 ; ligne < this.Lignes ; ligne++ )
            {
                for ( int colonne = 0 ; colonne < b.Colonnes ; colonne++ )
                {
                    for ( int d = 0 ; d < this.Colonnes ; d++ )
                    {
                        operationCount++;
                    }
                }
            }
            return operationCount;
        }
        /// <summary>
        /// Faire une multiplication pour un nombre indéfini de matrice
        /// </summary>
        /// <param name="b">Tableau de matrice à multiplier</param>
        /// <returns>Matrice résultante</returns>
        public Matrix Product(params Matrix[ ] b)
        {
            var a = this;
            for ( var m = 0 ; m < b.Length ; m++ )
            {
                a = a * b[m];
            }
            return a;
        }
        /// <summary>
        /// Faire une multiplication pour un nombre indéfini de matrice
        /// </summary>
        /// <param name="b">Tableau de matrice à multiplier</param>
        /// <param name="operationCount">Nombre d'opération ayant été nécessaire</param>
        /// <returns>Matrice résultante</returns>
        public Matrix Product(out int operationCount , params Matrix[ ] b)
        {
            operationCount = 0;

            var a = this;
            for ( var m = 0 ; m < b.Length ; m++ )
            {
                int operationIteration =0;
                a = a.Product(b[m]);
                operationCount += operationIteration;
            }
            return a;
        }
        /// <summary>
        /// Buffer permettant de ne pas recalculer le détermimant
        /// </summary>
        private Nombre? determinant=null;
        /// <summary>
        /// Permet de retourner le détermimant
        /// </summary>
        public Nombre Determinant
        {
            get
            {
                if ( determinant != null )
                    return determinant.Value;

                if ( !EstCarree )
                    throw new SizeMismatchException("La matrice doit etre carrée");
                if ( Lignes == 1 )
                    return ( Nombre ) matrix[0 , 0];
                if ( Lignes == 2 )
                {
                    return ( Nombre ) ( matrix[0 , 0] * matrix[1 , 1] - matrix[1 , 0] * matrix[0 , 1] );
                }
                else
                {
                    Nombre det=0;
                    for ( var s = 0 ; s < Lignes ; s++ )
                    {
                        det += this[0 , s] * GetComplementAlgebrique(0 , s);
                    }
                    return ( Nombre ) det;
                }
            }
        }
        /// <summary>
        /// Permet d'obtenir le complément algébrique pour une position X,Y
        /// </summary>
        /// <param name="p1">X</param>
        /// <param name="p2">Y</param>
        /// <returns>Complément algébrique</returns>
        public Nombre GetComplementAlgebrique(int p1 , int p2)
        {
            Matrix m= GetMineur(p1,p2);
            if ( ( p1 + p2 ) % 2 == 0 )
                return m.Determinant;
            else
                return -m.Determinant;
        }
        /// <summary>
        /// Permet d'obtenir la matrice mineur pour une position
        /// </summary>
        /// <param name="p1">X</param>
        /// <param name="p2">Y</param>
        /// <returns>Matrice mineur</returns>
        public Matrix GetMineur(int p1 , int p2)
        {
            if ( Lignes == 1 || Colonnes == 1 )
                return Matrix.Parse("[1]");
            Matrix m = new Matrix(Lignes-1,Colonnes-1);

            int x=0;
            int y=0;
            for ( var i = 0 ; i < Lignes ; i++ )
            {
                if ( i != p1 )
                {
                    y = 0;
                    for ( var j = 0 ; j < Lignes ; j++ )
                    {
                        if ( j != p2 )
                        {
                            m[x , y] = this[i , j];
                            y++;
                        }
                    }
                    x++;
                }
            }
            return m;
        }
        /// <summary>
        /// Retourne si la matrice est diagonale
        /// </summary>
        public bool EstDiagonale
        {
            get
            {
                return !EstStricte && EstTriangulaireInferieure && EstTriangulaireSuperieure;
            }
        }
        /// <summary>
        /// Vérifie que la matrice est triangulaire
        /// </summary>
        public bool EstTriangulaire { get { return EstTriangulaireInferieure || EstTriangulaireSuperieure; } }
        /// <summary>
        /// Vérifie que la matrice est stricte(diagonale=0)
        /// </summary>
        public bool EstStricte
        {
            get
            {
                if ( !EstCarree )
                    return false;
                for ( int i = 0 ; i < this.Lignes ; i++ )
                    if ( this[i , i] != 0 )
                        return false;
                return true;
            }
        }
        /// <summary>
        /// Permet de créer une matrice identité à partir d'une dimension
        /// </summary>
        /// <param name="v">Dimension</param>
        /// <returns>Matrice identité</returns>
        public static Matrix Identity(int v)
        {
            Matrix I = new Matrix(v , v);
            for ( var i = 0 ; i < v ; i++ )
                I[i , i] = 1;
            return I;
        }
        /// <summary>
        /// Vérifie qu'elle est triangulaire Supérieure
        /// </summary>
        public bool EstTriangulaireSuperieure
        {
            get
            {
                if ( !EstCarree ) return false;
                for ( var i = 0 ; i < this.Lignes ; i++ )
                {
                    for ( var j = 0 ; j < i - 1 ; j++ )
                    {
                        if ( this[i , j] != 0 ) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Vérifie qu'elle est trigulaire Inférieure
        /// </summary>
        public bool EstTriangulaireInferieure
        {
            get
            {
                if ( !EstCarree ) return false;
                for ( var i = 0 ; i < this.Lignes ; i++ )
                {
                    for ( var j = i + 1 ; j < this.Colonnes ; j++ )
                    {
                        if ( this[i , j] != 0 ) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Vérifie quelle est carrée
        /// </summary>
        public bool EstCarree
        {
            get
            {
                return Colonnes == Lignes;
            }
        }
        /// <summary>
        /// Vérifie qu'elle est une matrice identité
        /// </summary>
        public bool EstIdentite
        {
            get
            {
                if ( !EstCarree || EstStricte || !EstTriangulaireInferieure || !EstTriangulaireSuperieure )
                    return false;
                for ( int i = 0 ; i < this.Lignes ; i++ )
                    if ( this[i , i] != 1 )
                        return false;
                return true;
            }
        }
        /// <summary>
        /// Vérifie que c'est une matrice régulière
        /// </summary>
        public bool EstReguliere
        {
            get
            {
                return Determinant != 0;
            }
        }
        /// <summary>
        /// Permet d'obtenir la trace de la matrice
        /// </summary>
        public double Trace
        {
            get
            {
                if ( EstCarree )
                {
                    Nombre trace=0;
                    for ( var i = 0 ; i < Lignes ; i++ )
                        trace += matrix[i , i];
                    return ( double ) trace;
                }
                throw new SizeMismatchException("La matrice doit être carrée");
            }
        }
        /// <summary>
        /// Permet d'obtenir la transposee de la matrice
        /// </summary>
        public Matrix Transposee
        {
            get
            {
                Matrix b = new Matrix(Colonnes,Lignes);
                {
                    for ( var i = 0 ; i < Lignes ; i++ )
                    {
                        for ( var j = 0 ; j < Colonnes ; j++ )
                        {
                            b[j , i] = this[i , j];
                        }
                    }
                }
                return b;
            }
        }
        /// <summary>
        /// Permet de copier la matrice
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            return new Matrix(matrix);
        }
        /// <summary>
        /// Permet d'obtenir la CoMatrice
        /// </summary>
        public Matrix CoMatrice
        {
            get
            {
                Matrix com=new Matrix(Lignes,Colonnes);
                for ( var i = 0 ; i < Lignes ; i++ )
                    for ( var j = 0 ; j < Colonnes ; j++ )
                        com[i , j] = GetComplementAlgebrique(i , j);
                return com;
            }
        }
        /// <summary>
        /// Permet d'obtenir la matrice inverse
        /// </summary>
        public Matrix MatriceInverse
        {
            get
            {
                if ( !EstCarree )
                    throw new SizeMismatchException("La matrice doit être carrée");
                if ( Determinant == 0 )
                    throw new ArgumentException("Le déterminant ne peut être nul");

                return CoMatrice.Transposee / Determinant;
            }
        }
        /// <summary>
        /// Permet d'obtenir le nombre de lignes
        /// </summary>
        public int Lignes { get { return matrix.GetLength(0); } }
        /// <summary>
        /// Permet d'obtenir le nombre de colonnes
        /// </summary>
        public int Colonnes { get { return matrix.GetLength(1); } }
        /// <summary>
        /// Permet l'accès à une valeur de la matrice
        /// </summary>
        /// <param name="a">i</param>
        /// <param name="b">j</param>
        /// <returns>Valeur</returns>
        public Nombre this[int a , int b]
        {
            get { return matrix[a , b]; }
            set { matrix[a , b] = value; determinant = null; }
        }
        public static Matrix operator +(Matrix a , Matrix b)
        {
            return a.Add(b);
        }
        public static Matrix operator *(Matrix a , Matrix b)
        {
            return a.Product(b);
        }
        public static Matrix operator *(Matrix a , Nombre b)
        {
            return a.ScalarProduct(b);
        }
        public static Matrix operator /(Matrix a , Nombre b)
        {
            return a.ScalarProduct(1 / b);
        }
        public static Matrix operator *(Nombre a , Matrix b)
        {
            return b.ScalarProduct(a);
        }
        public static Matrix operator -(Matrix a)
        {
            Matrix b = new Matrix(a.Lignes,a.Colonnes);
            for ( var i = 0 ; i < a.Lignes ; i++ )
                for ( var j = 0 ; j < a.Colonnes ; j++ )
                    b[i , j] = -a[i , j];
            return b;
        }
        public static bool operator ==(Matrix a , Matrix b)
        {
            if ( a.Colonnes == b.Colonnes && a.Lignes == b.Lignes )
            {
                for ( var i = 0 ; i < a.Lignes ; i++ )
                {
                    for ( var j = 0 ; j < a.Colonnes ; j++ )
                    {
                        if ( a[i , j] != b[i , j] )
                            return false;
                    }
                }
                return true;
            }
            return false;
        }
        public static bool operator !=(Matrix a , Matrix b)
        {
            return !( a == b );
        }
        public override bool Equals(object obj)
        {
            if ( obj is Matrix )
            {
                return ( this == ( Matrix ) obj );
            }
            return false;
        }
        public override int GetHashCode()
        {
            return matrix.GetHashCode();
        }
        public static Matrix Parse(string s)
        {
            try
            {
                s = s.Replace("\r" , "");
                s = s.Replace("\n" , "");
                string[] lignes = s.Split(new char[] { ',' } );

                string[][] values = (from x in lignes
                                     select
                                     x.Replace("[","").Replace("]","")
                                     .Split(new char[ ] { ' ' } , StringSplitOptions.RemoveEmptyEntries))
                                     .ToArray();

                Nombre[,] matrice = new Nombre[lignes.Length,values[0].Length];
                for ( var i = 0 ; i < values.Length ; i++ )
                {
                    for ( var j = 0 ; j < values[i].Length ; j++ )
                    {
                        if ( values[i][j].Contains("/") )
                            matrice[i , j] = Nombre.Parse(values[i][j] , CultureInfo.InvariantCulture);
                        else
                            matrice[i , j] = double.Parse(values[i][j] , CultureInfo.InvariantCulture);
                    }
                }
                return new Matrix(matrice);
            }
            catch { throw new FormatException("Le format de la chaîne d'entrée ne correspond pas à une matrice"); }
        }

        public override string ToString()
        {
            return ToString(false);
        }
        public string ToString(bool enFraction)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[\r\n");
            for ( var i = 0 ; i < Lignes ; i++ )
            {
                for ( var j = 0 ; j < Colonnes ; j++ )
                {
                    string val = (enFraction)?matrix[i , j].ToString():((double)matrix[i,j]).ToString(CultureInfo.InvariantCulture);
                    sb.Append(val.PadLeft(5));
                    sb.Append(' ');
                }
                sb.Append(",\r\n");
            }

            sb[sb.Length - 3] = '\r';
            sb[sb.Length - 2] = '\n';
            sb[sb.Length - 1] = ']';
            return sb.ToString();
        }
    }
}
