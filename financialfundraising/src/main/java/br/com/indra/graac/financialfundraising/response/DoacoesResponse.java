package br.com.indra.graac.financialfundraising.response;

import java.util.Date;

public class DoacoesResponse {
	private String numPedido;
	private Date dataDoacao;
	private String valor;

	public String getNumPedido() {
		return numPedido;
	}

	public void setNumPedido(String numPedido) {
		this.numPedido = numPedido;
	}

	public Date getDataDoacao() {
		return dataDoacao;
	}

	public void setDataDoacao(Date dataDoacao) {
		this.dataDoacao = dataDoacao;
	}

	public String getValor() {
		return valor;
	}

	public void setValor(String valor) {
		this.valor = valor;
	}

}
