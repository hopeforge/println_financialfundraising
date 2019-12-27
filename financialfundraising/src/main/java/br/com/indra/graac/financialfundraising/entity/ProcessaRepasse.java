package br.com.indra.graac.financialfundraising.entity;



import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;





@Entity
@Table(name = "VW_PROCESSA_REPASSE")
public class ProcessaRepasse {
	
	
	
	@Column(name="SOMA")
	private float valor;
	
	@Id
	@Column(name = "ID_LOJA")
	private int idLoja;

	public float getValor() {
		return valor;
	}

	public void setValor(int valor) {
		this.valor = valor;
	}

	public int getIdLoja() {
		return idLoja;
	}

	public void setIdLoja(int idLoja) {
		this.idLoja = idLoja;
	}
	
}
