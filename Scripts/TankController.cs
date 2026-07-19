using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TankController : MonoBehaviour
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

    public TankIdentity tankIdentity;  // Adicione essa linha para referenciar o TankIdentity

    // Sistema de recarga
    private int bulletsFired = 0;       // Contador de balas disparadas
    public int maxBullets = 5;          // Máximo de balas por recarga
    public float reloadTime = 5f;       // Tempo de recarga em segundos
    private bool isReloading = false;   // Verifica se está recarregando

    // Variáveis para controle de estado do tanque
    private bool isAlive = true;

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

        // Controle de disparos com recarga
        if (Input.GetKeyDown(KeyCode.Space) && !isReloading)
        {
            if (bulletsFired < maxBullets)
            {
                Shoot();
                bulletsFired++;
                if (bulletsFired >= maxBullets) StartCoroutine(Reload());
            }
        }

        staminaBar.value = currentStamina;

        // Verifica se o tanque está fora da cena
        if (IsTankOutOfScene())
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bala"))
        {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            TakeDamage(bullet.damage);
        }
    }
}
    public void TakeDamage(float damage)
{
    // Verifica se o tanque está "vivo" antes de processar o dano (aplicável aos dois tanques)
    if (currentHealth <= 0) return;

    // Aplica o dano à vida atual
    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    healthBar.value = currentHealth;

    // Log para debugging
    Debug.Log(gameObject.name + " took damage: " + damage + ", Remaining Health: " + currentHealth);

    // Lógica de morte ou vitória
    if (currentHealth <= 0)
    {
        if (gameObject.CompareTag("ShooterTank1")) // Identificação pelo Tag
        {
            Die(); // Lógica de morte do Tank1
        }
        else if (gameObject.CompareTag("ShooterTank2"))
        {
            victoryUIManager.ShowVictory(this.gameObject); // Lógica de vitória do Tank2
            enabled = false; // Desativa o controle do Tank2
        }
    }
}
    

    private void Die()
    {
        if (!isAlive)
        {
            Debug.Log($"O tanque {gameObject.name} já foi declarado morto. Nenhuma ação adicional será tomada.");
            return;
        }

        isAlive = false;
        currentHealth = 0f; // Zera a vida quando morre

        Debug.Log($"O tanque {gameObject.name} morreu. Iniciando lógica de derrota e chamando VictoryUIManager.");

        // Exibe o UI de vitória para o outro tanque
        victoryUIManager.ShowVictory(this.gameObject);

        // Desativa o script para parar a atualização após a derrota
        enabled = false;

        Debug.Log($"O script de {gameObject.name} foi desativado após a morte.");
    }

    bool IsTankOutOfScene()
    {
        bool isOutOfScene = false;

        // Definir uma margem de erro para o eixo y (valor ajustado para -15f)
        float marginY = -15f;

        // Verificar a posição y para cada tanque de forma independente
        if (tankIdentity.tankType == TankIdentity.TankType.Tank1)
        {
            if (transform.position.y < marginY || transform.position.x < -40 || transform.position.x > 40)
            {
                isOutOfScene = true;
            }
        }
        else if (tankIdentity.tankType == TankIdentity.TankType.Tank2)
        {
            if (transform.position.y < -15f || transform.position.x < -40 || transform.position.x > 40)
            {
                isOutOfScene = true;
            }
            // Adicionando Debug.Log para garantir que a condição do Tank2 está sendo verificada
            Debug.Log($"Tank2 está sendo verificado. Posição Y: {transform.position.y}");
        }

        // Depuração detalhada
        Debug.Log($"{gameObject.name} está fora da cena? {isOutOfScene} | Posição atual: {transform.position} | Condições: y < {marginY}: {transform.position.y < marginY}, x < -40: {transform.position.x < -40}, x > 40: {transform.position.x > 40}");

        return isOutOfScene;
    }

    void MoveTank()
    {
        float moveInput = 0;

        if (Input.GetKey(KeyCode.A)) moveInput = -1;
        if (Input.GetKey(KeyCode.D)) moveInput = 1;

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
        if (Input.GetKey(KeyCode.S)) cannonAngle -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) cannonAngle += rotationSpeed * Time.deltaTime;

        cannonAngle = Mathf.Clamp(cannonAngle, 0f, 180f);
        cannon.localRotation = Quaternion.Euler(0f, 0f, cannonAngle);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = firePoint.right * bulletSpeed;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        bulletsFired = 0;
        isReloading = false;
    }
}