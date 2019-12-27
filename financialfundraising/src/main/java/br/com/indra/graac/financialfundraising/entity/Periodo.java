/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package br.com.indra.graac.financialfundraising.entity;

import java.io.Serializable;
import java.util.Date;
import java.util.List;

import javax.persistence.Basic;
import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.NamedQueries;
import javax.persistence.NamedQuery;
import javax.persistence.OneToMany;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlTransient;

/**
 *
 * @author scastroa
 */
@Entity
@Table(name = "PERIODO")
@XmlRootElement
@NamedQueries({
    @NamedQuery(name = "Periodo.findAll", query = "SELECT p FROM Periodo p"),
    @NamedQuery(name = "Periodo.findByIdPeriodo", query = "SELECT p FROM Periodo p WHERE p.idPeriodo = :idPeriodo"),
    @NamedQuery(name = "Periodo.findByAbertura", query = "SELECT p FROM Periodo p WHERE p.abertura = :abertura"),
    @NamedQuery(name = "Periodo.findByFechamento", query = "SELECT p FROM Periodo p WHERE p.fechamento = :fechamento"),
    @NamedQuery(name = "Periodo.findByPago", query = "SELECT p FROM Periodo p WHERE p.pago = :pago"),
    @NamedQuery(name = "Periodo.findByLinkPagamento", query = "SELECT p FROM Periodo p WHERE p.linkPagamento = :linkPagamento")})
public class Periodo implements Serializable {

    private static final long serialVersionUID = 1L;
    @Id
    @GeneratedValue
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_PERIODO")
    private Integer idPeriodo;
    @Basic(optional = false)
    @NotNull
    @Column(name = "ABERTURA")
    @Temporal(TemporalType.TIMESTAMP)
    private Date abertura;
    @Column(name = "FECHAMENTO")
    @Temporal(TemporalType.TIMESTAMP)
    private Date fechamento;
    @Basic(optional = false)
    @NotNull
    @Column(name = "PAGO")
    private boolean pago;
    @Size(max = 255)
    @Column(name = "LINK_PAGAMENTO")
    private String linkPagamento;
    @JoinColumn(name = "ID_LOJA", referencedColumnName = "ID_LOJA")
    @ManyToOne(optional = false)
    private Loja idLoja;
    @OneToMany(cascade = CascadeType.ALL, mappedBy = "idPeriodo")
    private List<Loja> lojaList;

    public Periodo() {
    }

    public Periodo(Integer idPeriodo) {
        this.idPeriodo = idPeriodo;
    }

    public Periodo(Integer idPeriodo, Date abertura, boolean pago) {
        this.idPeriodo = idPeriodo;
        this.abertura = abertura;
        this.pago = pago;
    }

    public Integer getIdPeriodo() {
        return idPeriodo;
    }

    public void setIdPeriodo(Integer idPeriodo) {
        this.idPeriodo = idPeriodo;
    }

    public Date getAbertura() {
        return abertura;
    }

    public void setAbertura(Date abertura) {
        this.abertura = abertura;
    }

    public Date getFechamento() {
        return fechamento;
    }

    public void setFechamento(Date fechamento) {
        this.fechamento = fechamento;
    }

    public boolean getPago() {
        return pago;
    }

    public void setPago(boolean pago) {
        this.pago = pago;
    }

    public String getLinkPagamento() {
        return linkPagamento;
    }

    public void setLinkPagamento(String linkPagamento) {
        this.linkPagamento = linkPagamento;
    }

    public Loja getIdLoja() {
        return idLoja;
    }

    public void setIdLoja(Loja idLoja) {
        this.idLoja = idLoja;
    }

    @XmlTransient
    public List<Loja> getLojaList() {
        return lojaList;
    }

    public void setLojaList(List<Loja> lojaList) {
        this.lojaList = lojaList;
    }

    @Override
    public int hashCode() {
        int hash = 0;
        hash += (idPeriodo != null ? idPeriodo.hashCode() : 0);
        return hash;
    }

    @Override
    public boolean equals(Object object) {
        // TODO: Warning - this method won't work in the case the id fields are not set
        if (!(object instanceof Periodo)) {
            return false;
        }
        Periodo other = (Periodo) object;
        if ((this.idPeriodo == null && other.idPeriodo != null) || (this.idPeriodo != null && !this.idPeriodo.equals(other.idPeriodo))) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return "br.com.indra.graac.financialfundraising.entity.Periodo[ idPeriodo=" + idPeriodo + " ]";
    }
    
}
