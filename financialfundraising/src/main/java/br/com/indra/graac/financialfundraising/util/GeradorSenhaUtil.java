package br.com.indra.graac.financialfundraising.util;

import org.springframework.security.crypto.keygen.KeyGenerators;

public class GeradorSenhaUtil {

	public static String gerarSenha() {
		
		String key = KeyGenerators.string().generateKey();
		return key.substring(0, 7).toUpperCase();
	}
	
}
