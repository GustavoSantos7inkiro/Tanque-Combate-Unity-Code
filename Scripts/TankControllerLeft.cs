using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tank2 : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 21f;
    public Transform cannon;
    public float moveSpeed = 5f;
    public float rotationSpeed = 30f;
    private float cannonAngle = 0f;

    public Slider staminaBar;           // Barra de stamina no UI
    public float maxStamina = 100f;     // Stamina máxima
    private float currentStamina;       // Stamina atual
    public float staminaConsumption = 10f;  // Consumo de stamina por segundo
    public float staminaRecovery = 5f;      // Recuperação de stamina por segundo
    public float lowStaminaSpeed = 2f;      // Velocidade reduzida com baixa stamina

    public Slider healthBar;           // Barra de vida no UI
    public float maxHealth = 100f;     // Vida máxima
    private float currentHealth;       // Vida atual

    public VictoryUIManager victoryUIManager;  // Referência ao VictoryUIManager

     public TankIdentity tankIdentity;

    
    public float yLimit = -15f; // Posição de "fora da cena" no eixo Y
    // Sistema de recarga
    private int bulletsFired = 0;       // Contador de balas disparadas
    public int maxBullets = 5;          // Máximo de balas por recarga
    public float reloadTime = 5f;       // Tempo de recarga em segundos
    private bool isReloading = false;   // Verifica se está recarregando

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = currentStamina;
    }

    void Update()
    {
        MoveTank();
        RotateCannon();

        if (Input.GetKeyDown(KeyCode.Return) && !isReloading)
        {
            if (bulletsFired < maxBullets)
            {
                Shoot();
                bulletsFired++;
                if (bulletsFired >= maxBullets) StartCoroutine(Reload());
            }
        }

        // Verifica se o Tank2 saiu da cena
        if (transform.position.y < yLimit)
        {
            victoryUIManager.ShowVictory(this.gameObject);  // Chama vitória de Tank1
            enabled = false;
        }

        staminaBar.value = currentStamina;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
            TakeDamage(10);
        }
    }

    void TakeDamage(float damage)
{
    // Aplica o dano à vida atual
    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    healthBar.value = currentHealth;

    // Log para debugging
    Debug.Log(gameObject.name + " took damage: " + damage + ", Remaining Health: " + currentHealth);

    // Lógica de vitória ou morte
    if (currentHealth <= 0)
    {
        victoryUIManager.ShowVictory(this.gameObject);  // Lógica de vitória do Tank2
        enabled = false;  // Desativa o controle do Tank2
    }
}

    
    void MoveTank()
    {
        float moveInput = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1;
        if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1;

        if (moveInput != 0 && currentStamina > 0)
        {
            currentStamina -= staminaConsumption * Time.deltaTime;
            transform.Translate(Vector2.right * moveInput * moveSpeed * Time.deltaTime);
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecovery * Time.deltaTime;
        }

        if (currentStamina <= 0)
        {
            transform.Translate(Vector2.right * moveInput * lowStaminaSpeed * Time.deltaTime);
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    void RotateCannon()
    {
        if (Input.GetKey(KeyCode.DownArrow)) cannonAngle -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) cannonAngle += rotationSpeed * Time.deltaTime;

        cannonAngle = Mathf.Clamp(cannonAngle, 0f, 180f);
        cannon.localRotation = Quaternion.Euler(0f, 0f, -cannonAngle);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = -firePoint.right * bulletSpeed;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsFired = 0;
        isReloading = false;
    }
}