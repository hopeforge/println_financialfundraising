/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package br.com.indra.graac.financialfundraising.entity;

import java.io.Serializable;
import java.util.Date;

import javax.persistence.Basic;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.NamedQueries;
import javax.persistence.NamedQuery;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;
import javax.xml.bind.annotation.XmlRootElement;

/**
 *
 * @author scastroa
 */
@Entity
@Table(name = "DOACOES")
@XmlRootElement
@NamedQueries({
    @NamedQuery(name = "Doacoes.findAll", query = "SELECT d FROM Doacoes d"),
    @NamedQuery(name = "Doacoes.findByIdDoacao", query = "SELECT d FROM Doacoes d WHERE d.idDoacao = :idDoacao"),
    @NamedQuery(name = "Doacoes.findByNumPedido", query = "SELECT d FROM Doacoes d WHERE d.numPedido = :numPedido"),
    @NamedQuery(name = "Doacoes.findByDataDoacao", query = "SELECT d FROM Doacoes d WHERE d.dataDoacao = :dataDoacao"),
    @NamedQuery(name = "Doacoes.findByValor", query = "SELECT d FROM Doacoes d WHERE d.valor = :valor"),
    @NamedQuery(name = "Doacoes.findByIdLoja", query = "SELECT d FROM Doacoes d WHERE d.idLoja = :idLoja"),
    @NamedQuery(name = "Doacoes.findByIdPeriodo", query = "SELECT d FROM Doacoes d WHERE d.idPeriodo = :idPeriodo")})
public class Doacoes implements Serializable {

    private static final long serialVersionUID = 1L;
    @Id
    @GeneratedValue(strategy=GenerationType.IDENTITY)
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_DOACAO")
    private Integer idDoacao;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 255)
    @Column(name = "NUM_PEDIDO")
    private String numPedido;
    @Column(name = "DATA_DOACAO")
    @Temporal(TemporalType.TIMESTAMP)
    private Date dataDoacao;
    // @Max(value=?)  @Min(value=?)//if you know range of your decimal fields consider using these annotations to enforce field validation
    @Column(name = "VALOR")
    private Double valor;
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_LOJA")
    private int idLoja;
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_PERIODO")
    private int idPeriodo;
    @JoinColumn(name = "ID_DOADOR", referencedColumnName = "ID_DOADOR")
    @ManyToOne(optional = false)
    private Doador idDoador;

    public Doacoes() {
    }

    public Doacoes(Integer idDoacao) {
        this.idDoacao = idDoacao;
    }

    public Doacoes(Integer idDoacao, String numPedido, int idLoja, int idPeriodo) {
        this.idDoacao = idDoacao;
        this.numPedido = numPedido;
        this.idLoja = idLoja;
        this.idPeriodo = idPeriodo;
    }

    public Integer getIdDoacao() {
        return idDoacao;
    }

    public void setIdDoacao(Integer idDoacao) {
        this.idDoacao = idDoacao;
    }

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

    public Double getValor() {
        return valor;
    }

    public void setValor(Double valor) {
        this.valor = valor;
    }

    public int getIdLoja() {
        return idLoja;
    }

    public void setIdLoja(int idLoja) {
        this.idLoja = idLoja;
    }

    public int getIdPeriodo() {
        return idPeriodo;
    }

    public void setIdPeriodo(int idPeriodo) {
        this.idPeriodo = idPeriodo;
    }

    public Doador getIdDoador() {
        return idDoador;
    }

    public void setIdDoador(Doador idDoador) {
        this.idDoador = idDoador;
    }

    @Override
    public int hashCode() {
        int hash = 0;
        hash += (idDoacao != null ? idDoacao.hashCode() : 0);
        return hash;
    }

    @Override
    public boolean equals(Object object) {
        // TODO: Warning - this method won't work in the case the id fields are not set
        if (!(object instanceof Doacoes)) {
            return false;
        }
        Doacoes other = (Doacoes) object;
        if ((this.idDoacao == null && other.idDoacao != null) || (this.idDoacao != null && !this.idDoacao.equals(other.idDoacao))) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return "br.com.indra.graac.financialfundraising.entity.Doacoes[ idDoacao=" + idDoacao + " ]";
    }
    
}
