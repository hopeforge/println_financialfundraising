package br.com.indra.graac.financialfundraising.components;

import org.springframework.stereotype.Component;

@Component
public class DestinatarioEmail {

	private String nome;
	private String valorDoacao;
	private String email;
	private String senha;

	public DestinatarioEmail() {
	}

	public DestinatarioEmail(String nome, String valorDoacao, String email,String senha) {
		this.nome = nome;
		this.valorDoacao = valorDoacao;
		this.email = email;
	    this.senha = senha;
	}

	public String getNome() {
		return nome;
	}

	public String getValorDoacao() {
		return valorDoacao;
	}

	public String getEmail() {
		return email;
	}

	public String getSenha() {
		return senha;
	}
	

}