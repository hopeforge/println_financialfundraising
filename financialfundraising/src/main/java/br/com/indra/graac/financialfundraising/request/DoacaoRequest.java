package br.com.indra.graac.financialfundraising.request;

public class DoacaoRequest {

	private String valorDoacao;
	private String email;
	private String token;
	private String cpf;
	private String nome;

	public String getValorDoacao() {
		return valorDoacao;
	}

	public void setValorDoacao(String valorDoacao) {
		this.valorDoacao = valorDoacao;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getToken() {
		return token;
	}

	public void setIdLoja(String token) {
		this.token = token;
	}

	public String getCpf() {
		return cpf;
	}

	public void setCpf(String cpf) {
		this.cpf = cpf;
	}

	public String getNome() {
		return nome;
	}

	public void setNome(String nome) {
		this.nome = nome;
	}
	
}
