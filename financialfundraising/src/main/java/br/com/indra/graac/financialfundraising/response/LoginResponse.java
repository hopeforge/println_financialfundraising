package br.com.indra.graac.financialfundraising.response;

import java.util.List;

public class LoginResponse {
	private String nomeDoador;
    private String cpf;
    private String email;
    private List<DoacoesResponse> doacoes;
    
    
	public String getNomeDoador() {
		return nomeDoador;
	}
	public void setNomeDoador(String nomeDoador) {
		this.nomeDoador = nomeDoador;
	}
	public String getCpf() {
		return cpf;
	}
	public void setCpf(String cpf) {
		this.cpf = cpf;
	}
	public String getEmail() {
		return email;
	}
	public void setEmail(String email) {
		this.email = email;
	}
	public List<DoacoesResponse> getDoacoes() {
		return doacoes;
	}
	public void setDoacoes(List<DoacoesResponse> doacoes) {
		this.doacoes = doacoes;
	}
    
}
