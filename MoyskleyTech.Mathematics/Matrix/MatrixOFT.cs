using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyskleyTech.Mathematics
{
    using System.Globalization;
    using System.Reflection;

    public class Matrix<Number>
        where Number : struct
    {
        //private double[,] matrice;
        private Number[,] matrix;
        //Cree une matrice a partir d'une taille
        public Matrix(int line , int cols)
        {
            matrix = new Number[line , cols];
        }
        //Copie une matrice
        public Matrix(Number[ , ] matrix)
        {
            this.matrix = ( Number[ , ] ) matrix.Clone();
        }
        /// <summary>
        /// Addition matricielle
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Matrix<Number> Add(Matrix<Number> b)
        {
            if ( !( this.Columns == b.Columns && this.Rows == b.Rows ) )
                throw new SizeMismatchException("Les deux matrices n'ont pas la meme dimmension");
            Matrix<Number> c = new Matrix<Number>(Rows,Columns);
            for ( var i = 0; i < this.Rows; i++ )
            {
                for ( var j = 0; j < this.Columns; j++ )
                {
                    c[i , j] = ( dynamic ) this[i , j] + b[i , j];
                }
            }
            return c;
        }

        public Matrix<Number> Gauss()
        {
            Matrix<Number> res = Clone();
            Matrix<Number> operation = new Matrix<Number>(Rows,Columns+1);

            //Pour mettre des 0 en bas des pivots
            for ( var lignePivot = 0; lignePivot < res.Rows - 1; lignePivot++ )
            {
                //Trouver le pivot pour la ligne
                int positionPivot = res.GaussFindPivotForLine(lignePivot);
                if ( positionPivot == -1 )
                    throw new InvalidOperationException("Impossible de trouver un pivot");
                operation *= ( dynamic ) 0;
                for ( var ligne = lignePivot + 1; ligne < res.Rows; ligne++ )
                {
                    Number facteur = (dynamic)res.matrix[ligne,positionPivot]/res.matrix[lignePivot,positionPivot];
                    operation[ligne , Columns] = facteur;
                    for ( var colonne = 0; colonne < res.Columns; colonne++ )
                    {
                        operation[ligne , colonne] = -( dynamic ) res.matrix[lignePivot , colonne] * facteur;
                        res.matrix[ligne , colonne] = ( dynamic ) res.matrix[lignePivot , colonne] * facteur - res.matrix[ligne , colonne];
                    }
                }
            }
            //Pour mettre des 0 en haut des pivots et mettre la diagonale à 1
            for ( var lignePivot = res.Rows - 1; lignePivot >= 0; lignePivot-- )
            {
                int positionPivot = res.GaussFindPivotForLine(lignePivot);
                if ( positionPivot == -1 )
                    throw new InvalidOperationException("Impossible de trouver un pivot");
                operation *= ( dynamic ) 0;

                Number facteurDiv = res.matrix[lignePivot,positionPivot];
                operation[lignePivot , Columns] = facteurDiv;
                for ( var j = 0; j < res.Columns; j++ )
                {
                    operation[lignePivot , j] = facteurDiv;
                    res.matrix[lignePivot , j] /= ( dynamic ) facteurDiv;
                }

                for ( var ligne = lignePivot - 1; ligne >= 0; ligne-- )
                {
                    Number facteur = res.matrix[ligne,positionPivot];
                    operation[ligne , Columns] = facteur;
                    for ( var colonne = 0; colonne < res.Columns; colonne++ )
                    {
                        operation[ligne , colonne] = -( dynamic ) res.matrix[lignePivot , colonne] * facteur;
                        res.matrix[ligne , colonne] = ( dynamic ) res.matrix[lignePivot , colonne] * facteur - res.matrix[ligne , colonne];
                    }
                }
            }

            return res;
        }
        /// <summary>
        /// Permet d'augmenter une matrice pour permettre gauss
        /// </summary>
        /// <param name="b">Matrice B</param>
        /// <returns>Matrice augmentée</returns>
        public Matrix<Number> Augment(Matrix<Number> b)
        {
            Matrix<Number> a = this;
            if ( a.Rows != b.Rows )
                throw new SizeMismatchException("Les deux matrices n'ont pas le meme nombre de ligne");
            Matrix<Number> c = new Matrix<Number>(a.Rows, a.Columns+b.Columns);
            for ( var i = 0; i < a.Rows; i++ )
            {
                for ( var j = 0; j < a.Columns; j++ )
                {
                    c[i , j] = a[i , j];
                }
                for ( var j = 0; j < b.Columns; j++ )
                {
                    c[i , j + a.Columns] = b[i , j];
                }
            }
            return c;
        }
        /// <summary>
        /// Permet d'identifier un pivot de gauss pour une ligne
        /// </summary>
        /// <param name="lignePivot">Numéro de ligne</param>
        /// <returns>Numéro de colonne du pivot</returns>
        private int GaussFindPivotForLine(int lignePivot)
        {
            for ( var i = 0; i < Columns; i++ )
            {
                if ( ( dynamic ) matrix[lignePivot , i] != 0 )
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Produit entre matrice et nombre
        /// </summary>
        /// <param name="b">Le nombre</param>
        /// <returns>Matrice résultante</returns>
        public Matrix<Number> ScalarProduct(Number b)
        {
            Matrix<Number> c = new Matrix<Number>(Rows,Columns);
            for ( var i = 0; i < this.Rows; i++ )
            {
                for ( var j = 0; j < this.Columns; j++ )
                {
                    c[i , j] = ( dynamic ) this[i , j] * b;
                }
            }
            return c;
        }
        /// <summary>
        /// Fait un produit de 2 matrices
        /// </summary>
        /// <param name="b">Deuxième matrice à multiplier</param>
        /// <returns>Combinaison Résultat et nombre d'opération</returns>
        public Duo<Matrix<Number> , int> Product(Matrix<Number> b)
        {
            int operationCount = 0;
            if ( this.Columns != b.Rows )
                throw new SizeMismatchException("Le nombre de ligne de la matrice A n'est pas égal au nombre de colonne de la matrice B");
            Matrix<Number> c = new Matrix<Number>(this.Rows,b.Columns);

            for ( int i = 0; i < c.Rows; i++ )
            {
                for ( int j = 0; j < c.Columns; j++ )
                {
                    for ( int d = 0; d < this.Columns; d++ )
                    {
                        c[i , j] += ( dynamic ) this[i , d] * b[d , j];
                        operationCount++;
                    }
                }
            }
            return new Duo<Matrix<Number> , int>(c , operationCount);
        }
        /// <summary>
        /// Calcul le nombre d'opération pour un produit par rapport à une matrice B
        /// </summary>
        /// <param name="b">Deuxième matrice</param>
        /// <returns>Nombre opération</returns>
        public int ProductOperationCount(Matrix<Number> b)
        {
            var operationCount = 0;
            if ( this.Columns != b.Rows )
                throw new SizeMismatchException("Le nombre de ligne de la matrice A n'est pas égal au nombre de colonne de la matrice B");

            for ( int ligne = 0; ligne < this.Rows; ligne++ )
            {
                for ( int colonne = 0; colonne < b.Columns; colonne++ )
                {
                    for ( int d = 0; d < this.Columns; d++ )
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
        public Matrix<Number> Product(params Matrix<Number>[ ] b)
        {
            var a = this;
            for ( var m = 0; m < b.Length; m++ )
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
        public Matrix<Number> Product(out int operationCount , params Matrix<Number>[ ] b)
        {
            operationCount = 0;

            var a = this;
            for ( var m = 0; m < b.Length; m++ )
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
        private Number determinant=default(Number);
        /// <summary>
        /// Permet de retourner le détermimant
        /// </summary>
        public Number Determinant
        {
            get
            {
                if ( ( dynamic ) determinant != default(Number) )
                    return determinant;

                if ( !IsSquare )
                    throw new SizeMismatchException("La matrice doit etre carrée");
                if ( Rows == 1 )
                    return ( Number ) matrix[0 , 0];
                if ( Rows == 2 )
                {
                    return ( Number ) ( ( dynamic ) matrix[0 , 0] * matrix[1 , 1] - ( dynamic ) matrix[1 , 0] * matrix[0 , 1] );
                }
                else
                {
                    Number det=(dynamic)0;
                    for ( var s = 0; s < Rows; s++ )
                    {
                        det += ( dynamic ) this[0 , s] * AlgebricComplement(0 , s);
                    }
                    return ( Number ) det;
                }
            }
        }

        public Number AlgebricComplement(int p1 , int p2)
        {
            Matrix<Number> m= GetMinor(p1,p2);
            if ( ( p1 + p2 ) % 2 == 0 )
                return m.Determinant;
            else
                return -( dynamic ) m.Determinant;
        }

        public Matrix<Number> GetMinor(int p1 , int p2)
        {
            if ( Rows == 1 || Columns == 1 )
                return Parse("[1]");
            Matrix<Number> m = new Matrix<Number>(Rows-1,Columns-1);

            int x=0;
            int y=0;
            for ( var i = 0; i < Rows; i++ )
            {
                if ( i != p1 )
                {
                    y = 0;
                    for ( var j = 0; j < Rows; j++ )
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

        public bool IsDiagonal
        {
            get
            {
                return !IsStrict && IsInferiorTriangle && IsSuperiorTriangle;
            }
        }

        public bool IsTriangle { get { return IsInferiorTriangle || IsSuperiorTriangle; } }

        public bool IsStrict
        {
            get
            {
                if ( !IsSquare )
                    return false;
                for ( int i = 0; i < this.Rows; i++ )
                    if ( ( dynamic ) this[i , i] != 0 )
                        return false;
                return true;
            }
        }

        public static Matrix<Number> Identity(int v)
        {
            Matrix<Number> I = new Matrix<Number>(v , v);
            for ( var i = 0; i < v; i++ )
                I[i , i] = ( dynamic ) 1;
            return I;
        }

        public bool IsSuperiorTriangle
        {
            get
            {
                if ( !IsSquare ) return false;
                for ( var i = 0; i < this.Rows; i++ )
                {
                    for ( var j = 0; j < i - 1; j++ )
                    {
                        if ( ( dynamic ) this[i , j] != 0 ) return false;
                    }
                }
                return true;
            }
        }

        public bool IsInferiorTriangle
        {
            get
            {
                if ( !IsSquare ) return false;
                for ( var i = 0; i < this.Rows; i++ )
                {
                    for ( var j = i + 1; j < this.Columns; j++ )
                    {
                        if ( ( dynamic ) this[i , j] != 0 ) return false;
                    }
                }
                return true;
            }
        }

        public bool IsSquare
        {
            get
            {
                return Columns == Rows;
            }
        }

        public bool IsIdentity
        {
            get
            {
                if ( !IsSquare || IsStrict || !IsInferiorTriangle || !IsSuperiorTriangle )
                    return false;
                for ( int i = 0; i < this.Rows; i++ )
                    if ( ( dynamic ) this[i , i] != 1 )
                        return false;
                return true;
            }
        }
        public bool IsRegular
        {
            get
            {
                return ( dynamic ) Determinant != 0;
            }
        }
        public Number Trace
        {
            get
            {
                if ( IsSquare )
                {
                    Number trace=(dynamic)0;
                    for ( var i = 0; i < Rows; i++ )
                        trace += ( dynamic ) matrix[i , i];
                    return trace;
                }
                throw new SizeMismatchException("La matrice doit être carrée");
            }
        }
        public Matrix<Number> Transposed
        {
            get
            {
                Matrix<Number> b = new Matrix<Number>(Columns,Rows);
                {
                    for ( var i = 0; i < Rows; i++ )
                    {
                        for ( var j = 0; j < Columns; j++ )
                        {
                            b[j , i] = this[i , j];
                        }
                    }
                }
                return b;
            }
        }
        public Matrix<Number> Clone()
        {
            return new Matrix<Number>(matrix);
        }
        public Matrix<Number> Comatrix
        {
            get
            {
                Matrix<Number> com=new Matrix<Number>(Rows,Columns);
                for ( var i = 0; i < Rows; i++ )
                    for ( var j = 0; j < Columns; j++ )
                        com[i , j] = AlgebricComplement(i , j);
                return com;
            }
        }
        /// <summary>
        /// Permet d'obtenir la matrice inverse
        /// </summary>
        public Matrix<Number> Inverted
        {
            get
            {
                if ( !IsSquare )
                    throw new SizeMismatchException("La matrice doit être carrée");
                if ( ( dynamic ) Determinant == 0 )
                    throw new ArgumentException("Le déterminant ne peut être nul");

                return ( dynamic ) Comatrix.Transposed / Determinant;
            }
        }
        /// <summary>
        /// Permet d'obtenir le nombre de lignes
        /// </summary>
        public int Rows { get { return matrix.GetLength(0); } }
        /// <summary>
        /// Permet d'obtenir le nombre de colonnes
        /// </summary>
        public int Columns { get { return matrix.GetLength(1); } }
        /// <summary>
        /// Permet l'accès à une valeur de la matrice
        /// </summary>
        /// <param name="a">i</param>
        /// <param name="b">j</param>
        /// <returns>Valeur</returns>
        public Number this[int a , int b]
        {
            get { return matrix[a , b]; }
            set { matrix[a , b] = value; determinant = default(Number); }
        }
        public static Matrix<Number> operator +(Matrix<Number> a , Matrix<Number> b)
        {
            return a.Add(b);
        }
        public static Matrix<Number> operator *(Matrix<Number> a , Matrix<Number> b)
        {
            return a.Product(b);
        }
        public static Matrix<Number> operator *(Matrix<Number> a , Number b)
        {
            return a.ScalarProduct(b);
        }
        public static Matrix<Number> operator /(Matrix<Number> a , Number b)
        {
            return a.ScalarProduct(( dynamic ) 1 / b);
        }
        public static Matrix<Number> operator *(Number a , Matrix<Number> b)
        {
            return b.ScalarProduct(a);
        }
        public static Matrix<Number> operator -(Matrix<Number> a)
        {
            Matrix<Number> b = new Matrix<Number>(a.Rows,a.Columns);
            for ( var i = 0; i < a.Rows; i++ )
                for ( var j = 0; j < a.Columns; j++ )
                    b[i , j] = -( dynamic ) a[i , j];
            return b;
        }
        public static bool operator ==(Matrix<Number> a , Matrix<Number> b)
        {
            if ( a.Columns == b.Columns && a.Rows == b.Rows )
            {
                for ( var i = 0; i < a.Rows; i++ )
                {
                    for ( var j = 0; j < a.Columns; j++ )
                    {
                        if ( ( dynamic ) a[i , j] != b[i , j] )
                            return false;
                    }
                }
                return true;
            }
            return false;
        }
        public static bool operator !=(Matrix<Number> a , Matrix<Number> b)
        {
            return !( a == b );
        }
        public override bool Equals(object obj)
        {
            if ( obj is Matrix<Number> )
            {
                return ( this == ( Matrix<Number> ) obj );
            }
            return false;
        }
        public override int GetHashCode()
        {
            return matrix.GetHashCode();
        }
        public static Matrix<Number> Parse(string s)
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

                Number[,] matrice = new Number[lignes.Length,values[0].Length];
                for ( var i = 0; i < values.Length; i++ )
                {
                    for ( var j = 0; j < values[i].Length; j++ )
                    {
                        matrice[i , j] = ( Number ) typeof(Number).GetRuntimeMethod("Parse" , new Type[ ] { typeof(string) }).Invoke(null , new object[ ] { values[i][j] });
                    }
                }
                return new Matrix<Number>(matrice);
            }
            catch { throw new FormatException("Le format de la chaîne d'entrée ne correspond pas à une matrice"); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[\r\n");
            for ( var i = 0; i < Rows; i++ )
            {
                for ( var j = 0; j < Columns; j++ )
                {
                    string val = matrix[i , j].ToString();
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
        public Matrix<Number> InsertRow(int rloc , params Number[ ] row)
        {
            return InsertRow(rloc , ( IEnumerable<Number> ) row);
        }
        public Matrix<Number> InsertRow(int rloc , IEnumerable<Number> row)
        {
            var arow = row.ToArray();
            Matrix<Number> m = new Matrix<Number>(Rows+1,Columns);
            for ( var r = 0; r < Rows + 1; r++ )
            {
                for ( var c = 0; c < Columns; c++ )
                {
                    if ( r < rloc )
                        m[r , c] = this[r , c];
                    else if ( r == rloc )
                        m[r , c] = arow[c];
                    else
                        m[r , c] = this[r - 1 , c];
                }
            }
            return m;
        }
        public Matrix<Number> InsertColumn(int cloc , params Number[ ] col)
        {
            return InsertColumn(cloc , ( IEnumerable<Number> ) col);
        }
        public Matrix<Number> InsertColumn(int cloc , IEnumerable<Number> col)
        {
            var acol = col.ToArray();
            Matrix<Number> m = new Matrix<Number>(Rows,Columns+1);
            for ( var r = 0; r < Rows; r++ )
            {
                for ( var c = 0; c < Columns + 1; c++ )
                {
                    if ( c < cloc )
                        m[r , c] = this[r , c];
                    else if ( c == cloc )
                        m[r , c] = acol[r];
                    else
                        m[r , c] = this[r , c - 1];
                }
            }
            return m;
        }
        public IEnumerable<Number> GetColumn(int v)
        {
            return ( from x in Enumerable.Range(0 , Rows) select matrix[x , v] );
        }
        public IEnumerable<Number> GetLine(int v)
        {
            return ( from y in Enumerable.Range(0 , Columns) select matrix[v , y] );
        }

        public IEnumerable<Number> AllValuesByRow()
        {
            for ( var r = 0; r < Rows; r++ )
            {
                for ( var c = 0; c < Columns; c++ )
                {
                    yield return this[r , c];
                }
            }
        }
        public IEnumerable<Number> AllValuesByColumn()
        {
            for ( var c = 0; c < Columns; c++ )
            {
                for ( var r = 0; r < Rows; r++ )
                {
                    yield return this[r , c];
                }
            }
        }
        public Matrix<Number> Feed(params Number[ ] feed)
        {
            return Feed(( IEnumerable<Number> ) feed);
        }
        public Matrix<Number> Feed(IEnumerable<Number> feed)
        {
            int p=0;
            int l=0;
            foreach ( var n in feed )
            {
                this[l , p] = n;
                p++; if ( p == Columns ) { p = 0; l++; }
            }

            return this;
        }
        public static explicit operator Matrix(Matrix<Number> p)
        {
            var mat = new Matrix(p.matrix.GetLength(0),p.matrix.GetLength(1));
            mat.Feed(p.AllValuesByRow().Cast<dynamic>().Select((x)=>(double)x));
            return mat;
        }
    }
}
