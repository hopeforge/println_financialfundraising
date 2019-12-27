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
import javax.persistence.GenerationType;
import javax.persistence.Id;
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
@Table(name = "DOADOR")
@XmlRootElement
@NamedQueries({
    @NamedQuery(name = "Doador.findAll", query = "SELECT d FROM Doador d"),
    @NamedQuery(name = "Doador.findByIdDoador", query = "SELECT d FROM Doador d WHERE d.idDoador = :idDoador"),
    @NamedQuery(name = "Doador.findByNomeDoador", query = "SELECT d FROM Doador d WHERE d.nomeDoador = :nomeDoador"),
    @NamedQuery(name = "Doador.findByCpf", query = "SELECT d FROM Doador d WHERE d.cpf = :cpf"),
    @NamedQuery(name = "Doador.findByEmail", query = "SELECT d FROM Doador d WHERE d.email = :email"),
    @NamedQuery(name = "Doador.findByCPFSenha", query = "SELECT d FROM Doador d WHERE d.cpf = :cpf AND d.senha = :senha"),
    @NamedQuery(name = "Doador.findByCPFEmail", query = "SELECT d FROM Doador d WHERE d.cpf = :cpf AND d.email = :email")})
public class Doador implements Serializable {

    private static final long serialVersionUID = 1L;
    @Id
    @GeneratedValue(strategy=GenerationType.IDENTITY)
    @Basic(optional = false)
    @NotNull
    @Column(name = "ID_DOADOR")
    private Integer idDoador;
    @Basic(optional = false)
    @NotNull
    @Size(min = 1, max = 255)
    @Column(name = "NOME_DOADOR")
    private String nomeDoador;
    @Size(max = 11)
    @Column(name = "CPF")
    private String cpf;
    // @Pattern(regexp="[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", message="E-mail inválido")//if the field contains email address consider using this annotation to enforce field validation
    @Size(max = 255)
    @Column(name = "EMAIL")
    private String email;
    @Size(max = 100)
    @Column(name = "SENHA")
    private String senha;
    @OneToMany(cascade = CascadeType.ALL, mappedBy = "idDoador")
    private List<Doacoes> doacoesList;

    public Doador() {
    }

    public Doador(Integer idDoador) {
        this.idDoador = idDoador;
    }

    public Doador(Integer idDoador, String nomeDoador) {
        this.idDoador = idDoador;
        this.nomeDoador = nomeDoador;
    }

    public Integer getIdDoador() {
        return idDoador;
    }

    public void setIdDoador(Integer idDoador) {
        this.idDoador = idDoador;
    }

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

    @XmlTransient
    public List<Doacoes> getDoacoesList() {
        return doacoesList;
    }

    public void setDoacoesList(List<Doacoes> doacoesList) {
        this.doacoesList = doacoesList;
    }

    @Override
    public int hashCode() {
        int hash = 0;
        hash += (idDoador != null ? idDoador.hashCode() : 0);
        return hash;
    }

    @Override
    public boolean equals(Object object) {
        // TODO: Warning - this method won't work in the case the id fields are not set
        if (!(object instanceof Doador)) {
            return false;
        }
        Doador other = (Doador) object;
        if ((this.idDoador == null && other.idDoador != null) || (this.idDoador != null && !this.idDoador.equals(other.idDoador))) {
            return false;
        }
        return true;
    }

    @Override
    public String toString() {
        return "br.com.indra.graac.financialfundraising.entity.Doador[ idDoador=" + idDoador + " ]";
    }

	public String getSenha() {
		return senha;
	}

	public void setSenha(String senha) {
		this.senha = senha;
	}
    
}
