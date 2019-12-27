/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package br.com.indra.graac.financialfundraising.entity;

import java.io.Serializable;
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
import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlTransient;

/**
 *
 * @author scastroa
 */
@Entity
@Table(name = "LOJA")
@XmlRootElement
@NamedQueries({
    @NamedQuery(name = "Loja.findAll", query = "SELECT l FROM Loja l"),
    @NamedQuery(name = "Loja.findByIdLoja", query = "SELECT l FROM Loja l WHERE l.idLoja = :idLoja"),
    @NamedQuery(name = "Loja.findByNomeLoja", query = "SELECT l FROM Loja l WHERE l.nomeLoja = :nomeLoja"),
    @NamedQuery(name = "Loja.findByCnpj", query = "SELECT l FROM Loja l WHERE l.cnpj = :cnpj"),
    @NamedQuery(name = "Loja.findByEmail", query = "SELECT l FROM Loja l WHERE l.email = :email"),
    @NamedQuery(name = "Loja.findBySenha", query = "SELECT l FROM Loja l WHERE l.senha = :senha"),
    @NamedQuery(name = "Loja.findByToken", query = "SELECT l FROM Loja l WHERE l.token = :token")})
public class Loja implements Serializable {

    private static final long serialVersionUID = 1L;
    @Id
    @GeneratedValue
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_LOJA")
    private Integer idLoja;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 255)
    @Column(name = "NOME_LOJA")
    private String nomeLoja;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 14)
    @Column(name = "CNPJ")
    private String cnpj;
    // @Pattern(regexp="[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", message="E-mail inválido")//if the field contains email address consider using this annotation to enforce field validation
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 100)
    @Column(name = "EMAIL")
    private String email;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 100)
    @Column(name = "SENHA")
    private String senha;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 255)
    @Column(name = "TOKEN")
    private String token;
    @OneToMany(cascade = CascadeType.ALL, mappedBy = "idLoja")
    private List<Periodo> periodoList;
    @JoinColumn(name = "ID_PERIODO", referencedColumnName = "ID_PERIODO")
    @ManyToOne(optional = false)
    private Periodo idPeriodo;

    public Loja() {
    }

    public Loja(Integer idLoja) {
        this.idLoja = idLoja;
    }

    public Loja(Integer idLoja, String nomeLoja, String cnpj, String email, String senha, String token) {
        this.idLoja = idLoja;
        this.nomeLoja = nomeLoja;
        this.cnpj = cnpj;
        this.email = email;
        this.senha = senha;
        this.token = token;
    }

    public Integer getIdLoja() {
        return idLoja;
    }

    public void setIdLoja(Integer idLoja) {
        this.idLoja = idLoja;
    }

    public String getNomeLoja() {
        return nomeLoja;
    }

    public void setNomeLoja(String nomeLoja) {
        this.nomeLoja = nomeLoja;
    }

    public String getCnpj() {
        return cnpj;
    }

    public void setCnpj(String cnpj) {
        this.cnpj = cnpj;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getSenha() {
        return senha;
    }

    public void setSenha(String senha) {
        this.senha = senha;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    @XmlTransient
    public List<Periodo> getPeriodoList() {
        return periodoList;
    }

    public void setPeriodoList(List<Periodo> periodoList) {
        this.periodoList = periodoList;
    }

    public Periodo getIdPeriodo() {
        return idPeriodo;
    }

    public void setIdPeriodo(Periodo idPeriodo) {
        this.idPeriodo = idPeriodo;
    }

    @Override
    public int hashCode() {
        int hash = 0;
        hash += (idLoja != null ? idLoja.hashCode() : 0);
        return hash;
    }

    @Override
    public boolean equals(Object object) {
        // TODO: Warning - this method won't work in the case the id fields are not set
        if (!(object instanceof Loja)) {
            return false;
        }
        Loja other = (Loja) object;
        if ((this.idLoja == null && other.idLoja != null) || (this.idLoja != null && !this.idLoja.equals(other.idLoja))) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return "br.com.indra.graac.financialfundraising.entity.Loja[ idLoja=" + idLoja + " ]";
    }
    
}
