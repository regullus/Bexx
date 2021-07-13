#region Bibliotecas

using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Runtime.InteropServices;

#endregion

namespace Utils
{
    #region enum

    /// <summary>
    /// Usado para definir a ação que será executada.
    /// </summary>
    public enum TipoCriptografia
    {
        /// <summary>
        /// Criptografa o dado.
        /// </summary>
        Criptografa = 0,
        /// <summary>
        /// Descriptografa o dado.
        /// </summary>
        Descriptografa = 1
    }

    //Tabela anuncio_situacao - Se atualizar a tabela deve-se atualizar aqui
    public enum Situacao
    {
        Ativo = 1,
        Desativado = 2,
        Excluido = 3,
    }

    public enum Perfil
    {
        Master = 1,
        Administrador = 2,
        Usuario = 3,
        Streamer = 4,
        Publicitario = 5,
    }

    //Tabela rede_social - Se atualizar a tabela deve-se atualizar aqui
    public enum RedeSocial
    {
        Todas = 1,
        Twitch = 2,
        YouTube = 3,
        Facebook = 4,
        Instagram = 5,
        Twitter = 6,
    }
    
    public enum Idioma
    {
        Portugues = 1,
        Espanhol = 2,
        Ingles = 3
    }
    #endregion

    #region Funcoes

    public class Funcoes
    {
        public static string GetCurrentMethodName()
        {
            var stack = new System.Diagnostics.StackFrame(1);

            return stack.GetMethod().Name;
        }
    }

    #endregion

    #region Helpers
    public class Helpers
    {
        #region Operating System
        public static bool OsIsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool OsIsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool OsIsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        #endregion

        #region Geral

        /// <summary>
        /// Gera senha forte.
        /// </summary>
        /// <returns>Senha forte.</returns>
        /// <example><code>
        /// <value>C#</value>
        /// cpUtilities.<font color="#2B91AF">Gerais</font> funcoes = <font color="blue">new</font> cpUtilities.<font color="#2B91AF">Gerais</font>();
        /// <font color="blue">try</font>
        /// {
        ///     lblInformacoes.Text = <font color="#a31515">"Gerar senha forte: "</font> + funcoes.GerarSenha();
        /// }
        /// <font color="blue">catch</font> (<font color="#2B91AF">Exception </font>ex)
        /// {
        ///     lblErro.Text = ex.Message;
        /// }
        /// <font color="blue">finally</font>
        /// {
        ///	  funcoes = <font color="blue">null</font>;
        /// }
        /// </code>
        /// <code>
        /// <value>VB</value>
        /// </code>
        ///</example>
        public static string GerarSenha()
        {
            string strSenha = "";
            Random rndValor = new Random();
            byte[] bytEspeciais = { 33, 35, 36, 37, 38, 40, 41, 42, 63, 64 };
            try
            {
                for (int i = 0; i <= 7; i++)
                {
                    if (i == 0)
                    {
                        strSenha = Convert.ToChar(rndValor.Next(65, 90)).ToString();
                    }

                    if (i == 1)
                    {
                        strSenha += Convert.ToChar(bytEspeciais[rndValor.Next(0, 9)]).ToString();
                    }

                    if ((i >= 2) || (i <= 6) && (i != 0) && (i != 1))
                    {
                        strSenha += Convert.ToChar(rndValor.Next(97, 122)).ToString();
                    }

                    if (i == 7)
                    {
                        strSenha += Convert.ToChar(rndValor.Next(48, 57)).ToString();
                    }
                }
                return strSenha;
            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.GerarSenha]" + ex.Message, ex);
            }
            finally
            {
                rndValor = null;
            }
        }

        /// <summary>
        /// Gera uma chave.
        /// </summary>
        /// <returns>Gera uma chave tipo de instalação de Software DFGHWERTSDFGCDFV</returns>
        public static string Chave()
        {
            try
            {
                int intBytes = 8;
                RNGCryptoServiceProvider objRNG = new RNGCryptoServiceProvider();
                byte[] bytBuffer = new byte[intBytes];

                objRNG.GetBytes(bytBuffer);

                StringBuilder hexString = new StringBuilder(64);

                for (int counter = 0; counter < bytBuffer.Length; counter++)
                {
                    hexString.Append(String.Format("{0:X2}", bytBuffer[counter]));
                }
                string strRetorno = hexString.ToString();
                return strRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.GerarChave]" + ex.Message, ex);
            }
        }

        public static string MimiHtml(string htmlNormal)
        {
            string HtmlMinimized = Regex.Replace(htmlNormal, @"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}", "");
            HtmlMinimized = Regex.Replace(HtmlMinimized, @"[ \f\r\t\v]?([\n\xFE\xFF/{}[\];,<>*%&|^!~?:=])[\f\r\t\v]?", "$1");
            HtmlMinimized = HtmlMinimized.Replace(";\n", ";");
            HtmlMinimized = HtmlMinimized.Replace(@"\r\n", string.Empty);
            HtmlMinimized = HtmlMinimized.Replace(@"\r", string.Empty);
            HtmlMinimized = HtmlMinimized.Replace(@"\n", string.Empty);
            return HtmlMinimized;
        }

        public static string QBuild(StringBuilder sql)
        {
            //Convert querys do banco no formato StringBuilder para string
            //a query deve ter 4 espaços em branco para identação e deve sempre iniciar com 1 espaço para os não identados
            //Ex.: " Select" e "    id = @id"
            return sql.ToString().Replace("   ", "");
        }

        public static string ConverteAcentuacao(string valor)
        {
            valor = valor.Replace("á", @"\u00e1");
            valor = valor.Replace("à", @"\u00e0");
            valor = valor.Replace("â", @"\u00e2");
            valor = valor.Replace("ã", @"\u00e3");
            valor = valor.Replace("ä", @"\u00e4");
            valor = valor.Replace("Á", @"\u00c1");
            valor = valor.Replace("À", @"\u00c0");
            valor = valor.Replace("Â", @"\u00c2");
            valor = valor.Replace("Ã", @"\u00c3");
            valor = valor.Replace("Ä", @"\u00c4");
            valor = valor.Replace("é", @"\u00e9");
            valor = valor.Replace("è", @"\u00e8");
            valor = valor.Replace("ê", @"\u00ea");
            valor = valor.Replace("ê", @"\u00ea");
            valor = valor.Replace("É", @"\u00c9");
            valor = valor.Replace("È", @"\u00c8");
            valor = valor.Replace("Ê", @"\u00ca");
            valor = valor.Replace("Ë", @"\u00cb");
            valor = valor.Replace("í", @"\u00ed");
            valor = valor.Replace("ì", @"\u00ec");
            valor = valor.Replace("î", @"\u00ee");
            valor = valor.Replace("ï", @"\u00ef");
            valor = valor.Replace("Í", @"\u00cd");
            valor = valor.Replace("Ì", @"\u00cc");
            valor = valor.Replace("Î", @"\u00ce");
            valor = valor.Replace("Ï", @"\u00cf");
            valor = valor.Replace("ó", @"\u00f3");
            valor = valor.Replace("ò", @"\u00f2");
            valor = valor.Replace("ô", @"\u00f4");
            valor = valor.Replace("õ", @"\u00f5");
            valor = valor.Replace("ö", @"\u00f6");
            valor = valor.Replace("Ó", @"\u00d3");
            valor = valor.Replace("Ò", @"\u00d2");
            valor = valor.Replace("Ô", @"\u00d4");
            valor = valor.Replace("Õ", @"\u00d5");
            valor = valor.Replace("Ö", @"\u00d6");
            valor = valor.Replace("ú", @"\u00fa");
            valor = valor.Replace("ù", @"\u00f9");
            valor = valor.Replace("û", @"\u00fb");
            valor = valor.Replace("ü", @"\u00fc");
            valor = valor.Replace("Ú", @"\u00da");
            valor = valor.Replace("Ù", @"\u00d9");
            valor = valor.Replace("Û", @"\u00db");
            valor = valor.Replace("ç", @"\u00e7");
            valor = valor.Replace("Ç", @"\u00c7");
            valor = valor.Replace("ñ", @"\u00f1");
            valor = valor.Replace("Ñ", @"\u00d1");
            valor = valor.Replace("&", @"\u0026");
            valor = valor.Replace("'", @"\u0027");

            return valor;
        }

        /// <summary>
        /// Retorna a sigla do Idioma
        /// </summary>
        /// <param name="idIdima"></param>
        /// <returns></returns>
        public static string SiglaIdioma(int idIdioma)
        {
            switch (idIdioma)
            {
                case 1:
                    return "pt-BR";
                case 2:
                    return "es-ES";
                case 3:
                    return "en-US";
                default:
                    return "pt-BR";
            }
        }

        public static string RemoverAcentos(string strEntrada)
        {
            string strResult = strEntrada;

            if (!String.IsNullOrEmpty(strEntrada))
            {
                strResult = "";

                /** Troca os caracteres acentuados por não acentuados **/
                string[] acentos = new string[] { "ç", "Ç", "á", "é", "í", "ó", "ú", "ý", "Á", "É", "Í", "Ó", "Ú", "Ý", "à", "è", "ì", "ò", "ù", "À", "È", "Ì", "Ò", "Ù", "ã", "õ", "ñ", "ä", "ë", "ï", "ö", "ü", "ÿ", "Ä", "Ë", "Ï", "Ö", "Ü", "Ã", "Õ", "Ñ", "â", "ê", "î", "ô", "û", "Â", "Ê", "Î", "Ô", "Û" };
                string[] semAcento = new string[] { "c", "C", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "Y", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U", "a", "o", "n", "a", "e", "i", "o", "u", "y", "A", "E", "I", "O", "U", "A", "O", "N", "a", "e", "i", "o", "u", "A", "E", "I", "O", "U" };

                for (int i = 0; i < acentos.Length; i++)
                {
                    strEntrada = strEntrada.Replace(acentos[i], semAcento[i]);
                }

                /** Troca os caracteres especiais da string por "" **/
                string[] caracteresEspeciais = { "\\.", ",", "-", ":", "\\(", "\\)", "ª", "\\|", "\\\\", "°", " " };

                for (int i = 0; i < caracteresEspeciais.Length; i++)
                {
                    strEntrada = strEntrada.Replace(caracteresEspeciais[i], "");
                }

                /** Troca os espaços no início por "" **/
                strEntrada = strEntrada.Replace("^\\s+", "");
                /** Troca os espaços no início por "" **/
                strEntrada = strEntrada.Replace("\\s+$", "");
                /** Troca os espaços duplicados, tabulações e etc por  "" **/
                strEntrada = strEntrada.Replace("\\s+", "");

                //Caso sobre algo difertente
                string strPattern = @"(?i)[^0-9_a-záéíóúàèìòùâêîôûãõç\s]";
                string strReplacement = "";
                Regex rexNovo = new Regex(strPattern);
                strResult = rexNovo.Replace(strEntrada, strReplacement);

                //deve ser caixa baixa
                strResult = strResult.ToLower();
            }

            return strResult;
        }

        /// <summary>
        /// Formatar uma string CNPJ ou CPF
        /// </summary>
        /// <param name="documento">string documento sem formatacao</param>
        /// <returns>string documento formatada</returns>
        /// <example>Recebe '99999999999999' Devolve '99.999.999/9999-99'</example>
        /// <example>Recebe '99999999999' Devolve '999.999.999-99'</example>
        public static string FormataCNPJCPF(string documento)
        {
            string retorno = "";

            documento = documento.Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(@"\", "").Replace("-", "").Replace("_", "");

            switch (documento.Length)
            {
                case 14:
                    //CNPJ
                    retorno = Convert.ToUInt64(documento).ToString(@"00\.000\.000\/0000\-00");
                    break;
                case 11:
                    //CPF
                    retorno = Convert.ToUInt64(documento).ToString(@"000\.000\.000\-00");
                    break;
                default:
                    retorno = documento;
                    break;
            }
            return retorno;
        }

        public static int Base64Decode(string identificador)
        {
            string c_base62_digits = "0123456789aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
            string v_temp_convert_val = identificador;
            int v_length = v_temp_convert_val.Length;
            int v_iterator = v_length;
            string v_temp_char;
            int v_temp_int;
            int v_return_value = 0;
            int v_multiplier = 1;
            try
            {
                while (v_iterator > 0)
                {
                    v_temp_char = v_temp_convert_val.Substring(v_iterator - 1, 1);
                    v_temp_int = c_base62_digits.IndexOf(v_temp_char);
                    v_return_value = v_return_value + (v_temp_int * v_multiplier);
                    v_multiplier = v_multiplier * 62;
                    v_iterator = v_iterator - 1;
                }
            }
            catch (Exception ex)
            {
                //ToDo LogErro
                string erro = ex.Message;
            }

            return v_return_value;
        }

        public static string Base64Encode(int identificador)
        {
            string c_base62_digits = "0123456789aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";
            int v_modulo;
            int @v_temp_int = identificador;
            string v_temp_val = "";
            string v_temp_char;

            try
            {
                while (v_temp_int != 0)
                {
                    v_modulo = v_temp_int % 62;
                    v_temp_char = c_base62_digits.Substring(v_modulo, 1);
                    v_temp_val = v_temp_char + @v_temp_val;
                    v_temp_int = (v_temp_int / 62);
                }
            }
            catch (Exception ex)
            {
                //ToDo LogErro
                string erro = ex.Message;
            }
            if (identificador == 0)
            {
                v_temp_val = "0";
            }

            return v_temp_val;
        }

        public static int idRedeSocial(string redeSocial)
        {
            int idRedeSocial = 0;
            if (Enum.IsDefined(typeof(Utils.RedeSocial), redeSocial))
            {
                idRedeSocial = (int)Enum.Parse(typeof(Utils.RedeSocial), redeSocial);
            }
            return idRedeSocial;
        }

        #endregion

        #region Morpho

        /// <summary>
        /// Criptografia Simples
        /// </summary>
        /// <param name="strEnt">Palavra que deseja criptografar ou descriptografar.</param>
        /// <param name="strChave">Chave usada para a criptografia ou descriptografia.</param>
        /// <param name="tipo">Criptografa, Descriptografa.</param>
        /// <returns>Valor criptografado ou descriptografar.</returns>
        public static string Morpho(string strEnt, TipoCriptografia tipo)
        {
            int lngPE;
            int lngCh;
            string strTemp;
            string strRet = "";
            short intStep;
            string strChave = "Opdpdpep2@21";

            try
            {
                if (String.IsNullOrEmpty(strEnt))
                {
                    return "";
                }

                if (tipo == TipoCriptografia.Criptografa)
                {
                    intStep = 1;
                }
                else
                {
                    intStep = 3;
                }

                lngCh = 0;
                for (lngPE = 0; lngPE <= strEnt.Length - 1; lngPE += intStep)
                {
                    if (lngCh > strChave.Length - 1)
                    {
                        lngCh = 0;
                    }
                    if (tipo == TipoCriptografia.Criptografa)
                    {
                        strTemp = Convert.ToString(Convert.ToInt32(Convert.ToChar(strEnt.Substring(lngPE, 1))) ^ Convert.ToInt32(Convert.ToChar(strChave.Substring(lngCh, 1))));
                    }
                    else
                    {
                        strTemp = Convert.ToString(Convert.ToChar(Convert.ToInt32(strEnt.Substring(lngPE, 3)) ^ Convert.ToInt32(Convert.ToChar(strChave.Substring(lngCh, 1)))));
                    }

                    if (tipo == TipoCriptografia.Criptografa)
                    {
                        switch (strTemp.Length)
                        {
                            case 1:
                                strTemp = "00" + strTemp;
                                break;
                            case 2:
                                strTemp = "0" + strTemp;
                                break;
                        }
                    }
                    strRet = strRet + strTemp;
                    lngCh = lngCh + 1;
                }

                return strRet;
            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.Morpho]" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Criptografia Simples
        /// </summary>
        /// <param name="strEnt">Palavra que deseja criptografar ou descriptografar.</param>
        /// <param name="strChave">Chave usada para a criptografia ou descriptografia.</param>
        /// <param name="tipo">Criptografa, Descriptografa.</param>
        /// <returns>Valor criptografado ou descriptografar.</returns>
        public static string Morpho(string strEnt, string strChave, TipoCriptografia tipo)
        {
            int lngPE;
            int lngCh;
            string strTemp;
            string strRet = "";
            short intStep;

            try
            {
                if (tipo == TipoCriptografia.Criptografa)
                {
                    intStep = 1;
                }
                else
                {
                    intStep = 3;
                }

                lngCh = 0;
                for (lngPE = 0; lngPE <= strEnt.Length - 1; lngPE += intStep)
                {
                    if (lngCh > strChave.Length - 1)
                    {
                        lngCh = 0;
                    }
                    if (tipo == TipoCriptografia.Criptografa)
                    {
                        strTemp = Convert.ToString(Convert.ToInt32(Convert.ToChar(strEnt.Substring(lngPE, 1))) ^ Convert.ToInt32(Convert.ToChar(strChave.Substring(lngCh, 1))));
                    }
                    else
                    {
                        strTemp = Convert.ToString(Convert.ToChar(Convert.ToInt32(strEnt.Substring(lngPE, 3)) ^ Convert.ToInt32(Convert.ToChar(strChave.Substring(lngCh, 1)))));
                    }

                    if (tipo == TipoCriptografia.Criptografa)
                    {
                        switch (strTemp.Length)
                        {
                            case 1:
                                strTemp = "00" + strTemp;
                                break;
                            case 2:
                                strTemp = "0" + strTemp;
                                break;
                        }
                    }
                    strRet = strRet + strTemp;
                    lngCh = lngCh + 1;
                }

            }
            catch (Exception)
            {
                strRet = "";
                //throw new Exception("[Gerais.Morpho]" + ex.Message, ex);
            }

            return strRet;
        }

        #endregion

        #region Encrypt

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        #region Security

        /// <summary> 
        /// Verifica se há entradas maliciosas de SQL Injection 
        /// </summary> 
        /// <param name="strEntr">String a ser Verificada a entrada</param> 
        /// <returns>True se há tentativa ocorreu</returns> 
        /// <remarks> 
        /// </remarks> 
        /// <history> 
        /// Regulus 
        /// </history> 
        public static string VerificarSQLInjection(string strEntr)
        {
            string[] strMal = new string[22];
            string strAchou = "";
            short shtI = 0;
            strEntr = strEntr.ToUpper();

            try
            {
                strMal[0] = "--";
                //Comentario 
                strMal[1] = "SELECT";
                // retrieve rows from a table or view 
                strMal[2] = "DELETE";
                // delete rows of a table 
                strMal[3] = "INSERT";
                // create new rows in a table 
                strMal[4] = "UPDATE";
                // update rows of a table 
                strMal[5] = "DROP";
                // remove a user-defined aggregate function 
                strMal[6] = "ALTER";
                // add users to a group or remove users from a group 
                strMal[7] = "ANALYZE";
                // collect statistics about a database 
                strMal[8] = "BEGIN";
                // start a transaction block 
                strMal[9] = "COMMIT";
                // commit the current transaction 
                strMal[10] = "CREATE";
                // define a new aggregate function 
                strMal[11] = "DEALLOCATE";
                // remove a prepared query 
                strMal[12] = "DECLARE";
                // define a cursor 
                strMal[13] = "EXECUTE";
                // execute a prepared query 
                strMal[14] = "EXPLAIN";
                // show the execution plan of a statement 
                strMal[15] = "GRANT";
                // define access privileges 
                strMal[16] = "ROLLBACK";
                // abort the current transaction 
                strMal[17] = "TRANSACTION";
                // start a transaction block 
                strMal[18] = "TRUNCATE";
                // empty a table 
                strMal[19] = "<";
                // empty a table 
                strMal[20] = ">";
                // empty a table 

                for (shtI = 0; shtI <= 20; shtI++)
                {
                    if (strEntr.IndexOf(strMal[shtI]) >= 0)
                    {
                        strAchou = strMal[shtI];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.VerificarSQLInjection]" + ex.Message, ex);
            }
            return strAchou;
        }

        #endregion

        #region Datas

        /// <summary>
        /// Define o TimeZone que a aplicacao ira utilizar
        /// Não se deve utilizar DateTime.Now no sistema, deve-se substituir por essa função
        /// </summary>
        public static DateTime DateTimeTron
        {
            get
            {
                //Ver uma forma de buscar no banco
                int intDateTime = -3; //Brasilia é o default caso não haja no banco o timezone da aplicacao

                DateTime dtNowUTC = DateTime.UtcNow;
                dtNowUTC = dtNowUTC.AddHours(intDateTime);

                return dtNowUTC;

            }
        }

        /// <summary>
        /// Formata a data atual no formato YYYYMMDD.
        /// </summary>
        /// <returns>Data Atual.</returns>
        public static string DataAtual()
        {
            try
            {
                string strDataTrans;
                string strDataParc;
                DateTime dteData = DateTimeTron;
                int intAno = dteData.Year;
                int intMes = dteData.Month;
                int intDia = dteData.Day;
                strDataTrans = Convert.ToString(intAno);
                strDataParc = Convert.ToString(intMes);
                if (strDataParc.Length < 2)
                {
                    strDataParc = "0" + strDataParc;
                };
                strDataTrans += strDataParc;
                strDataParc = Convert.ToString(intDia);
                if (strDataParc.Length < 2)
                {
                    strDataParc = "0" + strDataParc;
                };
                strDataTrans += strDataParc;
                return strDataTrans;

            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.DataAtual]" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Retorna data atual no formato yyyyMMdd
        /// </summary>
        /// <returns></returns>
        public static int DataAtualInt
        {
            get
            {
                try
                {
                    return Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        /// <summary>
        /// Formata a data atual no formato YYYYMMDD.
        /// </summary>
        /// <returns>Data Atual + dias</returns>
        public static int DataTronAtualMaisDias(int intDias)
        {
            try
            {
                string strDataTrans;
                string strDataParc;
                DateTime dteData = DateTimeTron.AddDays(intDias);
                int intAno = dteData.Year;
                int intMes = dteData.Month;
                int intDia = dteData.Day;
                strDataTrans = Convert.ToString(intAno);
                strDataParc = Convert.ToString(intMes);
                if (strDataParc.Length < 2)
                {
                    strDataParc = "0" + strDataParc;
                };
                strDataTrans += strDataParc;
                strDataParc = Convert.ToString(intDia);
                if (strDataParc.Length < 2)
                {
                    strDataParc = "0" + strDataParc;
                };
                strDataTrans += strDataParc;
                return Convert.ToInt32(strDataTrans);

            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.DataAtual]" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Retorna data atual no formato yyyyMMdd
        /// </summary>
        /// <returns></returns>
        public static int DataAtualIntAddDias(int dias)
        {
            try
            {
                return Convert.ToInt32(DateTime.Now.AddDays(dias).ToString("yyyyMMdd"));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Converte Data 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Data no formato dd/mm/yyyy ou yyyymmdd</returns>
        /// <remarks></remarks>
        public static string DataValor(string strData)
        {
            string strRetorno = "";
            try
            {
                if (!string.IsNullOrEmpty(strData))
                {
                    if (strData.IndexOf("/") > 0)
                    {
                        strRetorno = strData.Substring(6, 4) + strData.Substring(3, 2) + strData.Substring(0, 2);
                    }
                    else
                    {
                        strRetorno = strData.Substring(6, 2) + "/" + strData.Substring(4, 2) + "/" + strData.Substring(0, 4);
                    }
                }
            }
            catch (Exception)
            {
                //Todo logErro
                strRetorno = strData;
            }
            return strRetorno;
        }

        /// <summary>
        /// Retorna a hora e minuto 
        /// em um formato especifico. 
        /// </summary>
        /// <param name="Hora"></param>
        /// <returns>Hora no formato HH:MM ou HHMM</returns>
        /// <remarks></remarks>
        public static string HoraMinutoValue(string strHora)
        {
            string strRetorno = "";
            try
            {
                if (!string.IsNullOrEmpty(strHora))
                {
                    if (strHora.Length == 2)
                    {
                        strRetorno = strHora + ":00";
                    }
                    else
                    {
                        if (strHora.IndexOf(":") > 0)
                        {
                            strRetorno = strHora.Substring(0, 2) + strHora.Substring(3, 2);
                        }
                        else
                        {
                            strRetorno = strHora.Substring(0, 2) + ":" + strHora.Substring(2, 2);
                        }
                    }
                }
                return strRetorno;
            }
            catch (Exception)
            {
                //ToDo LogErro
                strRetorno = strHora;
            }
            return strRetorno;
        }

        /// <summary>
        /// Retorna a hora a partir de uma string 
        /// no formato hh:mm ou hhmm. 
        /// </summary>
        /// <param name="Hora"></param>
        /// <returns>Hora no formato HH</returns>
        /// <remarks></remarks>
        public static string HoraValue(string strHora)
        {
            string strRetorno = "";
            try
            {
                if (!string.IsNullOrEmpty(strHora))
                {
                    strRetorno = (strHora.Substring(0, 2));
                }
                else
                {
                    strRetorno = strHora;
                }
            }
            catch (Exception)
            {
                //ToDo logErro
                strRetorno = strHora;
            }
            return strRetorno;
        }

        /// <summary>
        /// Retorna a data e hora formatada a partir
        /// de uma string no formato yyyymmdd hhmm
        /// </summary>
        /// <param name="dataHora"></param>
        /// <returns></returns>
        public static string DataHoraValue(string dataHora)
        {
            string strRetorno = "";
            try
            {
                string[] valores = dataHora.Split(' ');

                if (valores.Length == 2)
                    strRetorno = (DataValor(valores[0]) + " " + HoraMinutoValue(valores[1]));
                else
                    strRetorno = (string.Empty);
            }
            catch (Exception)
            {
                //ToDo logErro
                strRetorno = dataHora;
            }
            return strRetorno;
        }

        /// <summary>
        /// Converte Data 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Data no formato dd/mm/yyyy ou yyyymmdd</returns>
        /// <remarks></remarks>
        public static string ConverterData(string strData)
        {
            string strRetorno = "";
            try
            {
                if (!string.IsNullOrEmpty(strData))
                {
                    if (strData.Trim().Length > 0)
                    {
                        if (strData.IndexOf("/") > 0)
                        {
                            strRetorno = strData.Substring(6, 4) + strData.Substring(3, 2) + strData.Substring(0, 2);
                        }
                        else
                        {
                            strRetorno = strData.Substring(6, 2) + "/" + strData.Substring(4, 2) + "/" + strData.Substring(0, 4);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Todo LogErr;
                strRetorno = "";
            }

            return strRetorno;
        }

        /// <summary>
        /// Converte Data 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Data no formato dd/mm/yyyy ou yyyymmdd</returns>
        /// <remarks></remarks>
        public static string ConverterData(int intData)
        {
            string strData = "";
            string strRetorno = "";
            try
            {
                strData = Convert.ToString(intData);
                if (!string.IsNullOrEmpty(strData))
                {
                    if (strData.IndexOf("/") > 0)
                    {
                        strRetorno = strData.Substring(6, 4) + strData.Substring(3, 2) + strData.Substring(0, 2);
                    }
                    else
                    {
                        strRetorno = strData.Substring(6, 2) + "/" + strData.Substring(4, 2) + "/" + strData.Substring(0, 4);
                    }
                }
                return strRetorno;
            }
            catch (Exception)
            {
                //Todo LogErr;
                return "19000101";
            }
        }

        /// <summary>
        /// Converte Data 
        /// </summary>
        /// <param name="Data"></param>
        /// <returns>Data no formato dd/mm/yyyy ou yyyymmdd</returns>
        /// <remarks></remarks>
        public static string ConverterData(DateTime? dttDataEntrada)
        {
            //Caso data seja null
            DateTime semData = new DateTime(1900, 1, 1, 0, 0, 0);

            DateTime dttData = dttDataEntrada ?? semData;

            string strData = dttData.ToString("yyyyMMdd");
            string strRetorno = "";
            try
            {
                if (!string.IsNullOrEmpty(strData))
                {
                    if (strData.Trim().Length > 0)
                    {
                        if (strData.IndexOf("/") > 0)
                        {
                            strRetorno = strData.Substring(6, 4) + strData.Substring(3, 2) + strData.Substring(0, 2);
                        }
                        else
                        {
                            strRetorno = strData.Substring(6, 2) + "/" + strData.Substring(4, 2) + "/" + strData.Substring(0, 4);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Todo LogErr;
                strRetorno = "";
            }

            return strRetorno;
        }


        #endregion

        #region token
        public static string GenerateRefreshToken
        {
            get
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            }
        }
        #endregion

    }
    #endregion

    #region Validações
    public class Validacao
    {

        /// <summary> 
        /// Validacao de endereço IP 
        /// </summary> 
        /// <param name="strIP">IP</param> 
        /// <returns>True se validado</returns> 
        /// <remarks>Validacao de endereço IP</remarks> 
        /// <history> 
        /// Regulus 
        /// </history> 
        public static bool IP(string strIP)
        {
            try
            {
                Regex objRegEx = default(Regex);
                objRegEx = new Regex("^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
                bool returnBoolean = false;
                returnBoolean = objRegEx.IsMatch(strIP);
                return returnBoolean;
            }
            catch (Exception ex)
            {
                throw new Exception("[Gerais.IP]" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Varifica se o email é válido
        /// </summary>
        /// <param name="strIn">String a ser tratada</param>
        /// <returns>Retorna true se o email for válido</returns>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// Regulus
        /// </history>
        public static bool ValidarEmail(string strIn)
        {
            return Regex.IsMatch(strIn, "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
        }

        //Método que valida o CPF
        public static bool ValidaCPF(string vrCPF)
        {
            try
            {
                string valor = vrCPF.Replace(".", "");
                valor = valor.Replace("-", "");

                if (valor.Length != 11)
                    return false;

                bool igual = true;
                for (int i = 1; i < 11 && igual; i++)
                    if (valor[i] != valor[0])
                        igual = false;

                if (igual || valor == "12345678909")
                    return false;

                int[] numeros = new int[11];
                for (int i = 0; i < 11; i++)
                    numeros[i] = int.Parse(
                    valor[i].ToString());

                int soma = 0;
                for (int i = 0; i < 9; i++)
                    soma += (10 - i) * numeros[i];

                int resultado = soma % 11;
                if (resultado == 1 || resultado == 0)
                {
                    if (numeros[9] != 0)
                        return false;
                }
                else if (numeros[9] != 11 - resultado)
                    return false;

                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += (11 - i) * numeros[i];

                resultado = soma % 11;

                if (resultado == 1 || resultado == 0)
                {
                    if (numeros[10] != 0)
                        return false;

                }
                else
                    if (numeros[10] != 11 - resultado)
                    return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Método que valida o CNPJ 
        public static bool ValidaCNPJ(string vrCNPJ)
        {
            if (!String.IsNullOrEmpty(vrCNPJ))
            {
                string CNPJ = vrCNPJ.Replace(".", "");
                CNPJ = CNPJ.Replace("/", "");
                CNPJ = CNPJ.Replace("-", "");

                int[] digitos, soma, resultado;
                int nrDig;
                string ftmt;
                bool[] CNPJOk;

                ftmt = "6543298765432";
                digitos = new int[14];
                soma = new int[2];
                soma[0] = 0;
                soma[1] = 0;
                resultado = new int[2];
                resultado[0] = 0;
                resultado[1] = 0;
                CNPJOk = new bool[2];
                CNPJOk[0] = false;
                CNPJOk[1] = false;

                try
                {
                    for (nrDig = 0; nrDig < 14; nrDig++)
                    {
                        digitos[nrDig] = int.Parse(
                         CNPJ.Substring(nrDig, 1));
                        if (nrDig <= 11)
                            soma[0] += (digitos[nrDig] *
                            int.Parse(ftmt.Substring(
                              nrDig + 1, 1)));
                        if (nrDig <= 12)
                            soma[1] += (digitos[nrDig] *
                            int.Parse(ftmt.Substring(
                              nrDig, 1)));
                    }

                    for (nrDig = 0; nrDig < 2; nrDig++)
                    {
                        resultado[nrDig] = (soma[nrDig] % 11);
                        if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                            CNPJOk[nrDig] = (
                            digitos[12 + nrDig] == 0);

                        else
                            CNPJOk[nrDig] = (
                            digitos[12 + nrDig] == (
                            11 - resultado[nrDig]));

                    }

                    return (CNPJOk[0] && CNPJOk[1]);

                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Método que valida o Cep
        public static bool ValidaCep(string cep)
        {
            try
            {
                if (cep.Length == 8)
                {
                    cep = cep.Substring(0, 5) + "-" + cep.Substring(5, 3);
                    //txt.Text = cep;
                }
                return System.Text.RegularExpressions.Regex.IsMatch(cep, ("[0-9]{5}-[0-9]{3}"));

            }
            catch (Exception)
            {
                return false;
            }
        }

        //Método que valida o Email
        public static bool ValidaEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, ("(?<user>[^@]+)@(?<host>.+)"));
        }

        //Método que valida o Email
        public static bool ValidaSenhaForte(string senha)
        {
            //entre 6 e 12 caracteres?
            if (senha.Length < 6 || senha.Length > 12)
                return false;
            //Há numero?
            if (!senha.Any(c => char.IsDigit(c)))
                return false;
            //Há letra Maiuscula
            if (!senha.Any(c => char.IsUpper(c)))
                return false;
            //Há letra Minuscula
            if (!senha.Any(c => char.IsLower(c)))
                return false;
            //Há simbolo
            if (!senha.Any(c => char.IsSymbol(c)))
                return false;

            //há mais de 3 caracteres repetidos?
            var contadorRepetido = 0;
            var ultimoCaracter = '\0';
            foreach (var c in senha)
            {
                if (c == ultimoCaracter)
                    contadorRepetido++;
                else
                    contadorRepetido = 0;

                if (contadorRepetido == 3)
                    return false;

                ultimoCaracter = c;
            }
            return true;
        }
    }

    #endregion
}